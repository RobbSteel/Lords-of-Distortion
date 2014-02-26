using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Priority_Queue;

public class ArenaManager : MonoBehaviour {
	PowerPrefabs powerPrefabs;

	const float PLACEMENT_TIME = 15f; 
	const float FIGHT_COUNT_DOWN_TIME = 5f;
	const float POST_MATCH_TIME = 5f;
	SessionManager sessionManager;

	public List<Transform> spawnPositions;
	private List<Vector3> playerSpawnVectors;

	public bool finishgame = false;
	
	HeapPriorityQueue<PowerSpawn> allTimedSpawns;
	List<NetworkPlayer> playersReady;
	
	float beginTime;
	int? livePlayerCount;
	public GameObject lordScreenUI;  //lordScreen ref for tweening
	private bool played;
	bool sentMyPowers = false;
	bool powersSynchronized = false;
	
	private GameObject timer;
	private PowerSpawn prevYield;
	public GameObject alertSymbol;
	/*
     * Commenting out LordSpawnManager and other relating parts
     * as we are not using it anymore 
     */
	LordSpawnManager lordsSpawnManager;

    PlacementUI placementUI;

	[RPC]
	void NotifyBeginTime(float time){
		Debug.Log ("start timer");
		beginTime = time;
	}
	
	
	void ServerDeathHandler(NetworkPlayer player){
		PlayerStats deadPlayerStats = sessionManager.gameInfo.GetPlayerStats(player);
		deadPlayerStats.score += CalculateScore();
		//Tell everyone this player's scores.
		networkView.RPC("SynchronizeScores", RPCMode.Others, deadPlayerStats.score, player);
		if(livePlayerCount == 0){
			print ("No more players");
			//Sets a bool that will be checked by the timer script "Countdown" for game finishing
			finishgame = true;
		}
	}
	
	//Called only on the server.
	[RPC]
	void NotifyPlayerDied(NetworkMessageInfo info){
		livePlayerCount--;
		if(Network.isServer){
			ServerDeathHandler(info.sender);
		}
	}
	
	
	[RPC]
	void SynchronizeScores(int score, NetworkPlayer playerToScore){
		PlayerStats deadPlayerStats = sessionManager.gameInfo.GetPlayerStats(playerToScore);
		deadPlayerStats.score = score;
	}
	
	//Called locally on every client including server when the player you control dies.
	void LostPlayer(GameObject deadPlayer){
		networkView.RPC ("NotifyPlayerDied", RPCMode.Others);
		livePlayerCount--;
		if(Network.isServer){
			ServerDeathHandler(Network.player);
		}
		sessionManager.KillPlayer(deadPlayer);

		//brign up the dead player placement screen.
		placementUI.SwitchToLive(true);
		placementUI.enabled = true;
		PlayMenuTween(false);

	}
	
	/*Clients request this fucntion on the server for every power. After that the server tells every
             other player*/
	
	[RPC]
	void AddPowerSpawnLocally(int typeIndex, Vector3 position, float time){
		PowerSpawn requested =  new PowerSpawn();
		requested.type = (PowerType)typeIndex;
		requested.position = position;
		requested.spawnTime = time;
		allTimedSpawns.Enqueue(requested, time);
	}
	
	private void CheckIfAllSynchronized(){
		if(playersReady.Count == sessionManager.gameInfo.players.Count){
			foreach(PowerSpawn power in allTimedSpawns){
				networkView.RPC ("AddPowerSpawnLocally", RPCMode.Others,
				                 (int)power.type, power.position, power.spawnTime);
			}
			powersSynchronized = true;
			networkView.RPC("FinishedSettingPowers", RPCMode.Others);
		}
	}
	
	//Called on the server.
	[RPC]
	void SentAllMyPowers(NetworkMessageInfo info){
		playersReady.Add(info.sender);
		CheckIfAllSynchronized();
		print ("Server received power spawn from " + sessionManager.gameInfo.GetPlayerOptions(info.sender).username);
	}
	
	void SentAllMyPowers(){
		if(!Network.isServer){
			Debug.Log ("Don't call this function on the client");
			return;
		}
		playersReady.Add(Network.player);
		CheckIfAllSynchronized();
	}
	
	[RPC]
	void FinishedSettingPowers(){
		powersSynchronized = true;
	}


	void Awake(){
		beginTime = float.PositiveInfinity;
		SetUpLordScreenTween();
		SetUpTimer();
		playerSpawnVectors = new List<Vector3>();

		foreach(Transform location in spawnPositions){
			playerSpawnVectors.Add(location.position);
		}
		playersReady = new List<NetworkPlayer>();
		allTimedSpawns = new HeapPriorityQueue<PowerSpawn>(30);
		sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
		placementUI = GetComponent<PlacementUI>();
		powerPrefabs = GetComponent<PowerPrefabs>();
	}
	
	// Use this for initialization
	void Start () {
		//this wont print because finishedloading is only true once all the start functions are called
		//in every object of the scnene.
		if(sessionManager.finishedLoading)
			Debug.Log("ready");
	}


	void OnEnable(){
		//add our function as an event to player
		/*http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
         *http://unity3d.com/learn/tutorials/modules/intermediate/scripting/events*/
		Controller2D.onDeath += LostPlayer;
		PowerSlot.powerKey += SpawnTriggerPower;
		placementUI.spawnNow += SpawnTriggerPower;
	}

	
	void OnDisable(){
		Controller2D.onDeath -= LostPlayer;
		PowerSlot.powerKey -= SpawnTriggerPower;
		placementUI.spawnNow -= SpawnTriggerPower;
	}
	/*
       * 1. Load Level Geometry
       * 2. Allow players to place powers. (Spawn instance of Placement UI)
       * 3. Notify players of how much time they have to place powers.
       * 4. When time is over, submit power RPCs to server.
       * 5. Server tells all other players about spawn locations and times. (RPC methods should be as
       * generic as possible, but we'll need different ones if certain powers have extra parameters).
       * 6. Have an event queue similar to the one in NetworkController.
       * 7. Players themselves determine if they're hit or not.* 
	 */

	void SnarePlayers(){
		foreach(NetworkPlayer player in sessionManager.gameInfo.players){
			GameObject playerObject = sessionManager.gameInfo.GetPlayerGameObject(player);
			playerObject.GetComponent<Controller2D>().Snare();
		}
	}

	void FreePlayers(){
		foreach(NetworkPlayer player in sessionManager.gameInfo.players){
			GameObject playerObject = sessionManager.gameInfo.GetPlayerGameObject(player);
			playerObject.GetComponent<Controller2D>().FreeFromSnare();
		}
	}

	void OnNetworkLoadedLevel(){
		//Instantiate(LordsScreenPrefab, LordsScreenPrefab.position, Quaternion.rotation);
		
		if(Network.isServer){
			beginTime =  TimeManager.instance.time + PLACEMENT_TIME;
			networkView.RPC ("NotifyBeginTime", RPCMode.Others, beginTime);
			//spawn players immediately
			livePlayerCount = sessionManager.SpawnPlayers(playerSpawnVectors);
			SnarePlayers();
			//TODO: make this a UI text function. (fade in fade out quickly)
			print ("You may place pleliminary traps");
		}
		//TODO: have something that checks if all players have finished loading.
	}

	private void SpawnTimedTraps(){
		if(allTimedSpawns.Count != 0){
			float currentTime = TimeManager.instance.time;
			//print ("Current time " + currentTime + ", next trap time " + (beginTime +  allTimedSpawns.First.Priority));
			
			//Display yield sign .5 seconds before power spawns, and destroy it when power spawns
			if (currentTime + 1.0f >= beginTime + FIGHT_COUNT_DOWN_TIME  + allTimedSpawns.First.Priority && prevYield != allTimedSpawns.First)
			{
				prevYield = allTimedSpawns.First;
				GameObject yield_sign = (GameObject)Instantiate(Resources.Load("alert-sign"), allTimedSpawns.First.position, Quaternion.identity);
				Destroy(yield_sign, 1.0f);
			}

			if(currentTime >= beginTime + allTimedSpawns.First.Priority + FIGHT_COUNT_DOWN_TIME){
				PowerSpawn spawn = allTimedSpawns.Dequeue();
				//convert power type to an int, which is an index to the array of power prefabs.
				Instantiate (powerPrefabs.list[(int)spawn.type], spawn.position, Quaternion.identity);
			}
		}
	}

	//This is is called when a player presses one of the trigger keys.
	private void SpawnTriggerPower(PowerSpawn spawn, GameObject uiElement){

        float currentTime = TimeManager.instance.time;
		if (placementUI.selectedTriggers.Contains(spawn) && currentTime >= beginTime  + FIGHT_COUNT_DOWN_TIME)
        {
			//unitiliazed
			NetworkViewID newViewID = default(NetworkViewID);
			if(spawn.type == PowerType.EXPLOSIVE){
				//Needs a viewID so that bombs can RPC each other.
				newViewID = Network.AllocateViewID();
			}
			networkView.RPC("SpawnPowerLocally", RPCMode.Others, (int)spawn.type, spawn.position, spawn.direction, newViewID);
			SpawnPowerLocally(spawn, newViewID);
            //Remove from your inventory and  disable button 
            placementUI.selectedTriggers.Remove(spawn);
            placementUI.DestroyAlphaPower(spawn);
			if(uiElement.GetComponent<PowerSlot>() != null){
				Vector3 offscreen = uiElement.transform.position;
				offscreen.y -= 400f;
				TweenPosition.Begin(uiElement, 1f, offscreen);
				uiElement.GetComponent<PowerSlot>().enabled = false;
			}
		}
	}

	//http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.StartCoroutine.html
	//http://docs.unity3d.com/Documentation/ScriptReference/Coroutine.html
	//Spawn a warning sign, wait 1.5 seconds, then spawn power. All of these are done locally on every client.
	IEnumerator YieldThenPower(PowerSpawn spawn, NetworkViewID optionalViewID)
    {
		GameObject instantiatedSymbol = (GameObject)Instantiate(alertSymbol, spawn.position, Quaternion.identity);
        yield return new WaitForSeconds(0.7f);
		Destroy(instantiatedSymbol);
		GameObject power =  Instantiate (powerPrefabs.list[(int)spawn.type], spawn.position, Quaternion.identity) as GameObject;
		power.GetComponent<Power>().direction = spawn.direction;
		//If the networkview id is specified, apply it to the networkview of the new power
		if(!Equals(optionalViewID, default(NetworkViewID))){
			power.GetComponent<NetworkView>().viewID = optionalViewID;
		}
    }

	//this function converts parameters into a powerspawn object
	[RPC]
	void SpawnPowerLocally(int type, Vector3 position, Vector3 direction, NetworkViewID optionalViewID){
		//TODO: add networkgroup thing to bomb because it requires rpc calls.
		PowerSpawn requestedSpawn = new PowerSpawn();
		requestedSpawn.type = (PowerType)type;
		requestedSpawn.position = position;
		requestedSpawn.direction = direction;
		SpawnPowerLocally(requestedSpawn, optionalViewID);
	}

	//The function that actually starts the coroutine for spawning powers.
	void SpawnPowerLocally(PowerSpawn spawn, NetworkViewID optionalViewID){
		StartCoroutine(YieldThenPower(spawn, optionalViewID));
	}

	bool playersFreed = false;
	bool trapsEnabled = false;
	void Update () {

		if(sentMyPowers == false && TimeManager.instance.time >= beginTime){
            placementUI.DestroyPowers();
			placementUI.Disable();


			/*
			 * Don't tween away powers
			PlayMenuTween(true);

			*/

			//Synchronize traps to server.
            foreach(PowerSpawn power in  placementUI.selectedTraps){
                if(Network.isServer){
					AddPowerSpawnLocally((int)power.type, power.position, power.spawnTime);
				}
				else {
					networkView.RPC ("AddPowerSpawnLocally", RPCMode.Server,
					                 (int)power.type, power.position, power.spawnTime);
				}
			}

			if(Network.isServer)
				SentAllMyPowers();
			else
				networkView.RPC ("SentAllMyPowers", RPCMode.Server);
			
			sentMyPowers = true;

			
		}


		//the rest of the code doesn't run until powers are finalized.
		if(!powersSynchronized)
			return;

		if(!playersFreed){
			print ("5 seconds until proximity and remote activated traps are enabled.");
			FreePlayers();
			playersFreed = true;
		}
		if(!trapsEnabled && TimeManager.instance.time >= beginTime + FIGHT_COUNT_DOWN_TIME){
			print ("Traps are Enabled");

			//also bring back power placement
			placementUI.SwitchToLive(false);
			placementUI.Enable();
			placementUI.ShowTriggers();
			trapsEnabled = true;
		}
		
		//Spawn one timed trap per frame, locally.
		SpawnTimedTraps();

	}
	
	private void SetUpTimer(){
		timer = GameObject.Find("timer");
		timer.GetComponent<countdown>().powerPlaceTimer = PLACEMENT_TIME;
		timer.GetComponent<countdown>().fightCountdown = FIGHT_COUNT_DOWN_TIME;
		timer.GetComponent<countdown>().postmatchtimer = POST_MATCH_TIME;
	}
	
	private void SetUpLordScreenTween(){
        lordScreenUI = GameObject.Find("PlacementRoot").transform.Find("Container").gameObject;
	}
	
	//plays lords spawn menu tween and deactives the menu
	private void PlayMenuTween(bool forward){
		if(forward)
			lordScreenUI.gameObject.GetComponent<TweenPosition>().PlayForward();
		else
			lordScreenUI.gameObject.GetComponent<TweenPosition>().PlayReverse();
		Debug.Log("played tween");
	}

	//Calculates score based on the number of players remaining when you die, saves it in PSinfo
	public int CalculateScore(){
		
		int score = 0;
		
		if(livePlayerCount == 0){
			
			score += 10;
			
		} else if(livePlayerCount == 1){
			
			score += 8;
			
		} else if(livePlayerCount == 2){
			
			score += 6;
			
		} else if(livePlayerCount == 3){
			
			score += 4;
		}
		
		return score;
	}

    public float getBeginTime()
    {
        return beginTime;
    }

}

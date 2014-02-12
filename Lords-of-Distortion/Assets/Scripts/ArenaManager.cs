using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

public class ArenaManager : MonoBehaviour {
	public Transform[] Powers;
	
	const float PLACEMENT_TIME = 15f;
	const float FIGHT_COUNT_DOWN_TIME = 5f;
	const float POST_MATCH_TIME = 5f;
	SessionManager sessionManager;
	List<Vector3> playerSpawnLocations;
	public bool finishgame = false;
	
	HeapPriorityQueue<PowerSpawn> allSpawns;
	List<NetworkPlayer> playersReady;
	
	float beginTime;
	int? livePlayers;
	public GameObject lordScreenUI;  //lordScreen ref for tweening
	private bool played;
	bool sentMyPowers = false;
	bool powersSynchronized = false;
	
	private GameObject timer;
	private PowerSpawn prevYield;
	
	LordSpawnManager lordsSpawnManager;
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
		if(livePlayers == 0){
			print ("No more players");
			//Sets a bool that will be checked by the timer script "Countdown" for game finishing
			finishgame = true;
		}
	}
	
	//Called only on the server.
	[RPC]
	void NotifyPlayerDied(NetworkMessageInfo info){
		livePlayers--;
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
		livePlayers--;
		if(Network.isServer){
			ServerDeathHandler(Network.player);
		}
		sessionManager.KillPlayer(deadPlayer);
	}
	
	/*Clients request this fucntion on the server for every power. After that the server tells every
             other player*/
	
	[RPC]
	void AddPowerSpawnLocally(int typeIndex, Vector3 position, float time){
		PowerSpawn requested =  new PowerSpawn();
		requested.type = (PowerSpawn.PowerType)typeIndex;
		requested.position = position;
		requested.spawnTime = time;
		allSpawns.Enqueue(requested, time);
	}
	
	private void CheckIfAllSynchronized(){
		if(playersReady.Count == sessionManager.gameInfo.players.Count){
			print ("Spam every player with every powerinfo");
			
			foreach(PowerSpawn power in allSpawns){
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
	
	void OnEnable(){
		//add our function as an event to player
		/*http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
                     *http://unity3d.com/learn/tutorials/modules/intermediate/scripting/events
                     */
		Controller2D.onDeath += LostPlayer;
	}
	
	void OnDisable(){
		Controller2D.onDeath -= LostPlayer;
	}
	
	
	void Awake(){
		beginTime = float.PositiveInfinity;
		SetUpLordScreenTween();
		SetUpTimer();
		playerSpawnLocations = new List<Vector3>();
		playerSpawnLocations.Add(new Vector3(-9f, -1.5f, 0f));
		playerSpawnLocations.Add(new Vector3(9f, -1.5f, 0f));
		playerSpawnLocations.Add(new Vector3(-9f, 4.3f, 0f));
		playerSpawnLocations.Add (new Vector3(9f, 4.3f, 0f));
		playersReady = new List<NetworkPlayer>();
		allSpawns = new HeapPriorityQueue<PowerSpawn>(30);
		sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
	}
	
	// Use this for initialization
	void Start () {
		//this wont print because finishedloading is only true once all the start functions are called
		//in every object of the scnene.
		if(sessionManager.finishedLoading)
			Debug.Log("ready");
	}
	/*
             * 1. Load Level Geometry
             * 2. Allow players to place powers. (Spawn instance of Placement UI)
             * 3. Notify players of how much time they have to place powers.
             * 4. When time is over, submit power RPCs to server.
             * 5. Server tells all other players about spawn locations and times. (RPC methods should be as
             * generic as possible, but we'll need different ones if certain powers have extra parameters).
             * 6. Have an event queue similar to the one in NetworkController.
             * 7. Players themselves determine if they're hit or not.
             * */
	
	void OnNetworkLoadedLevel(){
		//Instantiate(LordsScreenPrefab, LordsScreenPrefab.position, Quaternion.rotation);
		
		if(Network.isServer){
			Debug.Log ("start timer");
			beginTime =  TimeManager.instance.time + PLACEMENT_TIME;
			networkView.RPC ("NotifyBeginTime", RPCMode.Others, beginTime);
			//TEMP spawn bomb
			Network.Instantiate(Resources.Load("TransferExplosive"), Vector3.zero, Quaternion.identity, 0);

		}
		//TODO: have something that checks if all players have finished loading.
	}
	
	// Update is called once per frame
	void Update () {
		if(sentMyPowers == false && TimeManager.instance.time >= beginTime){
			print("Time's up: current " +TimeManager.instance.time + " begin time " + beginTime);
			PlayMenuTween();
			lordsSpawnManager.enabled = false;
			//Finalize powers, after 3 or more seconds start match (so we have time to receive other players powers)
			if(!lordsSpawnManager.readyToSend)
				lordsSpawnManager.FinalizePowers(this.gameObject);
			lordsSpawnManager.DestroyPowers();
			foreach(PowerSpawn power in  lordsSpawnManager.powerSpawns.Values){
				if(Network.isServer){
					AddPowerSpawnLocally((int)power.type, power.position, power.spawnTime);
				}
				else {
					networkView.RPC ("AddPowerSpawnLocally", RPCMode.Server,
					                 (int)power.type, power.position, power.spawnTime);
				}
			}
			if(Network.isServer){
				SentAllMyPowers();
			}
			else
				networkView.RPC ("SentAllMyPowers", RPCMode.Server);
			
			sentMyPowers = true;
		}
		
		//the rest of the code doesn't run until powers are finalized.
		if(!powersSynchronized)
			return;
		
		if(livePlayers == null && Network.isServer){
			livePlayers = sessionManager.SpawnPlayers(playerSpawnLocations);
		}
		
		//Spawn one power per frame, locally.
		if(allSpawns.Count != 0){
			float currentTime = TimeManager.instance.time;
			print ("Current time " + currentTime + ", next trap time " + (beginTime +  allSpawns.First.Priority));
			
			
			//Is this spawning multiple times for 1 power? ALso yield is a reserved keyword.
			
			//Display yield sign .5 seconds before power spawns, and destroy it when power spawns
			if (currentTime + 1.0f >= beginTime + FIGHT_COUNT_DOWN_TIME + allSpawns.First.Priority && prevYield != allSpawns.First)
			{
				prevYield = allSpawns.First;
				GameObject yield_sign = (GameObject)Instantiate(Resources.Load("alert-sign"), allSpawns.First.position, Quaternion.identity);
				Destroy(yield_sign, 1.0f);
			}
			
			
			if(currentTime >= beginTime + allSpawns.First.Priority + FIGHT_COUNT_DOWN_TIME){
				PowerSpawn spawn = allSpawns.Dequeue();
				//convert power type to an int, which is an index to the array of power prefabs.
				print ("Spawning a " + spawn.type);
				Instantiate (Powers[(int)spawn.type], spawn.position, Quaternion.identity);
			}
		}
	}
	
	private void SetUpTimer(){
		timer = GameObject.Find("timer");
		timer.GetComponent<countdown>().powerPlaceTimer = PLACEMENT_TIME;
		timer.GetComponent<countdown>().fightCountdown = FIGHT_COUNT_DOWN_TIME;
		timer.GetComponent<countdown>().postmatchtimer = POST_MATCH_TIME;
	}
	
	private void SetUpLordScreenTween(){
		played = false;
		lordsSpawnManager = GameObject.Find ("UI Root").GetComponent<LordSpawnManager>();
		lordScreenUI = GameObject.Find( "LordsScreen" );
		lordScreenUI.gameObject.GetComponent<TweenPosition>().enabled = false;
	}
	
	//plays lords spawn menu tween and deactives the menu
	private void PlayMenuTween(){
		if( !played ){
			played = true;
			lordScreenUI.gameObject.GetComponent<TweenPosition>().eventReceiver = this.gameObject;
			lordScreenUI.gameObject.GetComponent<TweenPosition>().callWhenFinished = "DeactivateLordScreen";
			lordScreenUI.gameObject.GetComponent<TweenPosition>().enabled = true;
			
			Debug.Log("played tween");
		}
		
	}
	//Calculates score based on the number of players remaining when you die, saves it in PSinfo
	public int CalculateScore(){
		
		int score = 0;
		
		if(livePlayers == 0){
			
			score += 10;
			
		} else if(livePlayers == 1){
			
			score += 8;
			
		} else if(livePlayers == 2){
			
			score += 6;
			
		} else if(livePlayers == 3){
			
			score += 4;
		}
		
		return score;
	}
	
	
	
	void DeactivateLordScreen(){
		//lordScreenUI.SetActive( false );
		Debug.Log( "deactivate LS" );
	}
}

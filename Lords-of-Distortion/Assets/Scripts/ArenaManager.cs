using UnityEngine;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour {
	public Transform[] Powers;

	const float PLACEMENT_TIME = 15f; 
	SessionManager sessionManager;
	List<Vector3> playerSpawnLocations;

	SortedList<float, PowerSpawn> allSpawns;
	
	List<NetworkPlayer> playersReady;

	float beginTime;
	int? livePlayers;
	public GameObject lordScreenUI;  //lordScreen ref for tweening
	private bool played;
	bool sentMyPowers = false;
	bool powersSynchronized = false;

    public Texture2D yield;

	LordSpawnManager lordsSpawnManager;
	[RPC]
	void NotifyBeginTime(float time){
		Debug.Log ("start timer");
		beginTime = time;
	}

	[RPC]
	void NotifyPlayerDied(){
		livePlayers--;
		if(livePlayers == 0 && Network.isServer){
			print ("No more players");
			sessionManager.LoadNextLevel();
		}
	}

	/*Clients request this fucntion on the server for every power. After that the server tells every
	 other player*/

	[RPC]
	void AddPowerSpawnLocally(int typeIndex, Vector3 position, float time){
		PowerSpawn requested =  new PowerSpawn();
		requested.type = (PowerSpawn.PowerType)typeIndex;
		requested.position = position;
		requested.spawnTime = time;
		allSpawns.Add(time, requested);
	}

	private void CheckIfAllSynchronized(){
		if(playersReady.Count == sessionManager.gameInfo.players.Count){
			print ("Spam every player with every powerinfo");
			
			foreach(PowerSpawn power in allSpawns.Values){
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

	void LostPlayer(GameObject deadPlayer){
		networkView.RPC ("NotifyPlayerDied", RPCMode.Others);
		NotifyPlayerDied();
		sessionManager.KillPlayer(deadPlayer);
	}

	void Awake(){
		beginTime = float.PositiveInfinity;
		SetUpLordScreenTween();
		playerSpawnLocations = new List<Vector3>();
		playerSpawnLocations.Add(new Vector3(-3.16764f, -3.177613f, 0f));
		playerSpawnLocations.Add(new Vector3(3.35127f, -1.387209f, 0f));
		playerSpawnLocations.Add(new Vector3(0.5738465f, -1.387209f, 0f));
		playerSpawnLocations.Add (new Vector3(-3.315388f, -0.4170055f, 0f));
		allSpawns = new SortedList<float, PowerSpawn>();
		playersReady = new List<NetworkPlayer>();
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
			//print ("Current time " + currentTime + ", next trap time " + (beginTime +  allSpawns.Keys[0]));

            //Display yield sign .5 seconds before power spawns, and destroy it when power spawns
            if (currentTime + 0.5f >= beginTime + allSpawns.Keys[0])
            { 
                GameObject yield = (GameObject)Instantiate(Resources.Load("alert-sign"), allSpawns.Values[0].position, Quaternion.identity);
                Destroy(yield, 0.5f);
            }

            if(currentTime >= beginTime + allSpawns.Keys[0]){
				PowerSpawn spawn = allSpawns.Values[0];
                allSpawns.RemoveAt(0);
            	//convert power type to an int, which is an index to the array of power prefabs.
				print ("Spawning a " + spawn.type);
				Instantiate (Powers[(int)spawn.type], spawn.position, Quaternion.identity);
			}
		}
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

	void DeactivateLordScreen(){
		//lordScreenUI.SetActive( false );
		Debug.Log( "deactivate LS" );
	}
}

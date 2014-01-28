using UnityEngine;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour {
	public Transform[] Powers;

	const float PLACEMENT_TIME = 15f; 
	SessionManager sessionManager;
	List<Vector3> spawnLocations;

	SortedList<float, PowerSpawn> allSpawns;
	List<PowerSpawn> mySpawns;
	List<NetworkPlayer> playersReady;

	float beginTime;
	int livePlayers;
	public GameObject lordScreenUI;
	private bool played;
	bool sentMyPowers = false;
	bool powersFinalized = false;

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

	//Called on the server.
	[RPC]
	void SentAllMyPowers(NetworkMessageInfo info){
		playersReady.Add(info.sender);

		if(playersReady.Count == sessionManager.gameInfo.players.Count){
			print ("Spam every player with every powerinfo");
			foreach(PowerSpawn power in allSpawns.Values){
				networkView.RPC ("AddPowerSpawnLocally", RPCMode.Others,
				                 (int)power.type, power.position, power.spawnTime);
			}
			powersFinalized = true;
			networkView.RPC("FinishedSettingPowers", RPCMode.Others);
		}
	}

	[RPC]
	void FinishedSettingPowers(){
		powersFinalized = true;
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
		played = false;
		beginTime = float.PositiveInfinity;
		lordScreenUI = GameObject.Find( "LordsScreen" );
		lordScreenUI.gameObject.GetComponent<TweenAlpha>().enabled = false;
		spawnLocations = new List<Vector3>();
		spawnLocations.Add(new Vector3(-3.16764f, -3.177613f, 0f));
		spawnLocations.Add(new Vector3(3.35127f, -1.387209f, 0f));
		spawnLocations.Add(new Vector3(0.5738465f, -1.387209f, 0f));
		spawnLocations.Add (new Vector3(-3.315388f, -0.4170055f, 0f));
		allSpawns = new SortedList<float, PowerSpawn>();
		mySpawns = new List<PowerSpawn>();
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
			livePlayers = sessionManager.SpawnPlayers(spawnLocations);
			Debug.Log ("start timer");
			beginTime =  sessionManager.timeManager.time + PLACEMENT_TIME;
			networkView.RPC ("NotifyBeginTime", RPCMode.Others, beginTime);
		}
		//TODO: have something that checks if all players have finished loading.
	}
	
	// Update is called once per frame
	void Update () {
		if(sentMyPowers == false && sessionManager.timeManager.time >= beginTime){
			PlayMenuTween();
			//Finalize powers, after 3 or more seconds start match (so we have time to receive other players powers)
			foreach(PowerSpawn power in  mySpawns){
				networkView.RPC ("AddPowerSpawnLocally", RPCMode.Server, 
				                 (int)power.type, power.position, power.spawnTime);
			}
			sentMyPowers = true;
		}

		//the rest of the code doesn't run until powers are finalized.
		if(!powersFinalized)
			return;
		//Spawn one power per frame, locally.
		if(allSpawns.Count != 0){
			float currentTime = sessionManager.timeManager.time;

			if(currentTime >= currentTime + allSpawns.Keys[0]){
				PowerSpawn spawn = allSpawns.Values[0];
				allSpawns.RemoveAt(0);
				//convert power type to an int, which is an index to the array of power prefabs.
				Instantiate (Powers[(int)spawn.type], spawn.position, Quaternion.identity);
			}
		}
	}

	void PlayMenuTween(){
		if( !played ){
			played = true;
			lordScreenUI.gameObject.GetComponent<TweenAlpha>().enabled = true;
			
		}
	}
}

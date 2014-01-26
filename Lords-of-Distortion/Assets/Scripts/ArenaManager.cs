using UnityEngine;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour {
	const float PLACEMENT_TIME = 15f; 
	SessionManager sessionManager;
	List<Vector3> spawnLocations;
	float beginTime;
	int livePlayers;

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

	void OnEnable(){
		//add our function as an event to player
		/*http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
		 * http://unity3d.com/learn/tutorials/modules/intermediate/scripting/events
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
		spawnLocations = new List<Vector3>();
		spawnLocations.Add(new Vector3(-3.16764f, -3.177613f, 0f));
		spawnLocations.Add(new Vector3(3.35127f, -1.387209f, 0f));
		spawnLocations.Add(new Vector3(0.5738465f, -1.387209f, 0f));
		spawnLocations.Add (new Vector3(-3.315388f, -0.4170055f, 0f));
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
		//if(instanceManager.timeManager.time >= beginTime)
			//Debug.Log("Go!");
	}
}

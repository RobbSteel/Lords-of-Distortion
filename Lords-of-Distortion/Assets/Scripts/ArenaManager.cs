using UnityEngine;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour {
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
		 * */
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

	void OnNetworkLoadedLevel(){
		if(Network.isServer){
			livePlayers = sessionManager.SpawnPlayers(spawnLocations);
			Debug.Log ("start timer");
			beginTime =  sessionManager.timeManager.time + 5.0f;
			networkView.RPC ("NotifyBeginTime", RPCMode.Others, beginTime);
		}
		//TODO: have something that checks if all players have finished loading.
	}
	
	// Update is called once per frame
	void Update () {
		//if(beginTime <= instanceManager.timeManager.time)
			//Debug.Log("Go!");
	}
}

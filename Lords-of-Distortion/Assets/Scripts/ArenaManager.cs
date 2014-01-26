using UnityEngine;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour {
	SessionManager instanceManager;
	List<Vector3> spawnLocations;
	float beginTime;

	[RPC]
	void NotifyBeginTime(float time){
		Debug.Log ("start timer");
		beginTime = time;
	}

	void Awake(){
		beginTime = float.PositiveInfinity;
		spawnLocations = new List<Vector3>();
		spawnLocations.Add(new Vector3(-3.16764f, -3.177613f, 0f));
		spawnLocations.Add(new Vector3(3.35127f, -1.387209f, 0f));
		spawnLocations.Add(new Vector3(0.5738465f, -1.387209f, 0f));
		spawnLocations.Add (new Vector3(-3.315388f, -0.4170055f, 0f));
		instanceManager = GameObject.Find ("FakeLobbySpawner").GetComponent<SessionManager>();
	}

	// Use this for initialization
	void Start () {
		//this wont print because finishedloading is only true once all the start functions are called
		//in every object of the scnene.
		if(instanceManager.finishedLoading)
			Debug.Log("ready"); 
	}

	void OnNetworkLoadedLevel(){
		if(Network.isServer){
			beginTime = instanceManager.SpawnPlayers(spawnLocations);
			Debug.Log ("start timer");
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

using UnityEngine;
using System.Collections.Generic;

public class ArenaSpawnManager : MonoBehaviour {
	LobbyInstanceManager instanceManager;
	List<Vector3> spawnLocations;
	double beginTime;

	[RPC]
	void NotifyBeginTime(float time){
		Debug.Log ("start timer");
		beginTime = (double)time;
	}

	void Awake(){
		beginTime = double.PositiveInfinity;
		spawnLocations = new List<Vector3>();
		spawnLocations.Add(new Vector3(-3.16764f, -4.177613f, 0f));
		spawnLocations.Add(new Vector3(3.35127f, -2.387209f, 0f));
		spawnLocations.Add(new Vector3(0.5738465f, -2.387209f, 0f));
		instanceManager = GameObject.Find ("FakeLobbySpawner").GetComponent<LobbyInstanceManager>();
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
			networkView.RPC ("NotifyBeginTime", RPCMode.Others, (float)beginTime);
		}
		//TODO: have something that checks if all players have finished loading.
	}
	
	// Update is called once per frame
	void Update () {
		if(beginTime <= instanceManager.timeManager.time)
			Debug.Log("Go!");
	}
}

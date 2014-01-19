using UnityEngine;
using System.Collections.Generic;

public class ArenaSpawnManager : MonoBehaviour {
	LobbyInstanceManager instanceManager;
	List<Vector3> spawnLocations;

	void Awake(){
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
		if(Network.isServer)
			instanceManager.SpawnPlayers(spawnLocations);
		//TODO: have something that checks if all players have finished loading.
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

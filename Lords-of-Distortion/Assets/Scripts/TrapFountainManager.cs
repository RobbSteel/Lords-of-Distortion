using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This object should be initialized at the same time on all clients.
//That is, it should not depend on load time of scene or local time.

public class TrapFountainManager : MonoBehaviour {

	public List<Transform> potentialLocations;
	public PlacementUI placementUI;
	private bool activated;

	float spawnTime = 15f;
	float interval = 13f;

	System.Random random;

	public GameObject TrapFountainPrefab;

	void SpawnFountain(){
		Transform location = potentialLocations[random.Next(0, potentialLocations.Count - 1)];
		GameObject fountain = (GameObject)Instantiate (TrapFountainPrefab, location.position, Quaternion.identity);
		fountain.GetComponent<TrapFountain>().placementUI = placementUI;

	}

	public void SetFirstSpawnTime(float time){
		spawnTime = time;
		activated = true;
	}

	//Sets the seed for the random number generator
	public void SetSeed(int seed){
		random = new System.Random(seed);
	}

	void Awake(){
		activated = false;
	}

	// Update is called once per frame
	void Update () {
		if(activated){
			spawnTime = spawnTime - Time.deltaTime;
			if(spawnTime <= 0f){
				SpawnFountain();
				spawnTime += interval;
			}
		}
	}
}

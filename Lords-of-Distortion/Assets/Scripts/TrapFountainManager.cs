using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This object should be initialized at the same time on all clients.
//That is, it should not depend on load time of scene or local time.

public class TrapFountainManager : MonoBehaviour {

	public List<Transform> potentialLocations;
	private bool activated;

	float spawnTime = 15f;
	float interval = 13f;

	System.Random random;

	public GameObject TrapFountainPrefab;

	void SpawnFountain(){
		Transform location = potentialLocations[random.Next(0, potentialLocations.Count - 1)];
		Instantiate (TrapFountainPrefab, location.position, Quaternion.identity);
	}

	public void SetFirstSpawnTime(float time){
		spawnTime = time;
	}
	//Sets the seed for the random number generator
	public void SetSeed(int seed){
		random = new System.Random(seed);
		activated = true;
	}


	void Awake(){
		activated = false;
	}

	void Start () {
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

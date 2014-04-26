using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	private GameObject timer;

	public float spawnTimer = 0f;			// timer for hazard spawn
	public float spawnCooldownTimer = 3f;	// amount of time inbetween spawn
	public float speed = 0f;
	//private countdown countDown_myTimer;	// Reference to countdown script
	private float myTimer;
	//int countDown_CurrentTimer;
	//int? arenaMan_livePlayers;		// Reference to ArenaManager script
	public Rigidbody2D SpikeHazards;			// Prefab of hazard perfabs.
	public Timer count_down;

	void Awake()
	{

		//countDown_CurrentTimer = GetComponent<countdown> ().CurrentTimer;
		//count_down.CurrentTimer = this.GetComponent<countdown> ();
		//arenaMan_livePlayers = GetComponent<ArenaManager>().livePlayers;

	}

	void Start ()
	{
		// Start calling the Spawn function repeatedly after a delay .
		//InvokeRepeating("StartSpawningHazards", spawnDelay, spawnTime);
	}

	void Update()
	{
		timer = GameObject.Find ("timer");
		//countdown count_Down = GetComponent<countdown>();
		// If currentTimer == 2 and myTimer reaches 10...

		if (timer.GetComponent<Timer>().countDownTime >= 35) //&& countDown_CurrentTimer == 2 && arenaMan_livePlayers != 0)

			// Once spawnDelay reaches 0
			if (spawnTimer <= 0) 
				// Start killing everyone
				StartSpawningHazards();
			
			else
			spawnTimer -= Time.deltaTime;
				

	}
	

	public void StartSpawningHazards()
	{
		// Instantiate a random enemy.
		Rigidbody2D hazardInstance = Instantiate(SpikeHazards, transform.position, transform.rotation) as Rigidbody2D;
		hazardInstance.velocity = new Vector2(speed, 0);
		//int hazardIndex = Random.Range(0, hazards.Length);
		//Instantiate(hazards[hazardIndex], transform.position, transform.rotation);

		// cool down timer for spawning
		spawnTimer = spawnCooldownTimer;


	}
}

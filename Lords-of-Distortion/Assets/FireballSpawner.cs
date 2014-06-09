using UnityEngine;
using System.Collections;

public class FireballSpawner : MonoBehaviour {
	
	public GameObject StageFireball;

	float frequency = 6f;
	float timer = 0f;

	void Update()
	{
		if(timer <= 0f)
		{
			Instantiate(StageFireball, transform.position, StageFireball.transform.rotation);
			timer = frequency + timer; //add remaining time (negative or zero)
		}
		else 
			timer -= Time.deltaTime;
	}
}

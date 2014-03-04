using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TrapFountain : MonoBehaviour {

	public GameObject fountainParticles;
	public GameObject getPowerParticles;
	ParticleSystem getPowerPSystem;
	
	bool used = false;
	public PlacementUI placementUI;

	void Awake(){
	
		fountainParticles = (GameObject)Instantiate(fountainParticles, transform.position, Quaternion.identity);
		fountainParticles.GetComponent<ParticleSystem>().Play();

		getPowerParticles = (GameObject)Instantiate(getPowerParticles, transform.position, Quaternion.identity);
		getPowerPSystem = getPowerParticles.GetComponent<ParticleSystem>();
	}
	// Use this for initialization
	void Start () {
		Destroy(gameObject, 6f);
	}


	
	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player"){
			if(!used){
				placementUI.Resupply();
				used = true;
				audio.Play();
				getPowerPSystem.Play();
			}
		}
	}

	void OnDisable(){
		Destroy(fountainParticles);
		Destroy (getPowerParticles);
	}
}

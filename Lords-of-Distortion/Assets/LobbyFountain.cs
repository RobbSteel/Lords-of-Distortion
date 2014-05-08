using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LobbyFountain : MonoBehaviour {
	
	public GameObject fountainParticles;
	public GameObject getPowerParticles;
	public bool tutorialUse;
	public PowerType specifiedPower;
	public bool randomPowerSupply;
	ParticleSystem getPowerPSystem;
	
	bool used = false;
	public PlacementUI placementUI;
	
	void Awake(){
		

	}
	// Use this for initialization
	void Start () {
		fountainParticles = (GameObject)Instantiate(fountainParticles, transform.position, Quaternion.identity);
		fountainParticles.GetComponent<ParticleSystem>().Play();
		
		getPowerParticles = (GameObject)Instantiate(getPowerParticles, transform.position, Quaternion.identity);
		getPowerPSystem = getPowerParticles.GetComponent<ParticleSystem>();

		var ui = GameObject.Find("LobbyPlacement(Clone)");
		placementUI = ui.GetComponentInChildren<PlacementUI>();
	}
	
	
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player"){
			if(placementUI.CanResupply()){
				reSupplyPlayer();
			}
		}
	}

	void reSupplyPlayer(){
		if(randomPowerSupply)
			placementUI.Resupply();
		else
			placementUI.Resupply(specifiedPower);
		
		used = true;
		audio.Play();
		getPowerPSystem.Play();
	}
	
	void OnDisable(){
		Destroy(fountainParticles);
		Destroy (getPowerParticles);
	}
}
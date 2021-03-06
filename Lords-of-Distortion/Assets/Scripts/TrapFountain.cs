﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TrapFountain : MonoBehaviour {

	public GameObject fountainParticles;
	public GameObject getPowerParticles;
	public bool tutorialUse;
	public PowerType specifiedPower;
	public bool randomPowerSupply;
	ParticleSystem getPowerPSystem;
	public AudioClip gemResupply;
	//public AudioClip gemPing;
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
		if( !tutorialUse ){
			Destroy(gameObject , 6f);

		}
	}


	
	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player"){

			if( tutorialUse )
			placementUI = GameObject.Find( "OfflinePlacement(Clone)").GetComponent<PlacementUI>();

			if(!used && placementUI.CanResupply()){
				this.GetComponent<Animator>().enabled = true;
				reSupplyPlayer();
			}
		}
	}

	void reSupplyPlayer(){
		if(randomPowerSupply){
			placementUI.Resupply();
			placementUI.Resupply();
		}
		else
			placementUI.Resupply(specifiedPower);

			used = true;
			audio.PlayOneShot(gemResupply, 0.6f);
			getPowerPSystem.Play();
	}

	void OnDisable(){
		Destroy(fountainParticles);
		Destroy (getPowerParticles);
	}
}

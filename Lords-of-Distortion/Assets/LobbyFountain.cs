using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LobbyFountain : MonoBehaviour {
	
	public GameObject fountainParticles;
	public GameObject getPowerParticles;
	public bool tutorialUse;
	ParticleSystem getPowerPSystem;
	
	bool used = false;
	public LobbyPlacement placementUI;
	
	void Awake(){
		
		fountainParticles = (GameObject)Instantiate(fountainParticles, transform.position, Quaternion.identity);
		fountainParticles.GetComponent<ParticleSystem>().Play();
		
		getPowerParticles = (GameObject)Instantiate(getPowerParticles, transform.position, Quaternion.identity);
		getPowerPSystem = getPowerParticles.GetComponent<ParticleSystem>();

		var ui = GameObject.Find("LobbyPlacement(Clone)");
		placementUI = ui.GetComponentInChildren<LobbyPlacement>();
	}
	// Use this for initialization
	void Start () {

	}
	
	
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player"){
			if(placementUI.CanResupply()){
				placementUI.Resupply();
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
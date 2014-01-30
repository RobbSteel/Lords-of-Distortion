﻿using UnityEngine;
using System.Collections;

public class Melee : MonoBehaviour {
	
	private Controller2D controller;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.

	public float meleeTimer = 0;
	public float coolDownTimer = 0.5f;

	void Awake()
	{
		// Setting up the references.
		anim = transform.root.gameObject.GetComponent<Animator>();
		controller = transform.root.GetComponent<Controller2D>();
		this.GetComponent<BoxCollider2D>().enabled = false;

	}
	
	void Update ()
	{
		if (meleeTimer <= 0) {
			// If the fire button is pressed...
			if (Input.GetButtonDown ("Fire3") && !controller.stunned) {
				startMelee ();
			}
		}
		else
			meleeTimer -= Time.deltaTime;
	}

	private void startMelee(){
		// Enable Box Collider 2D
		this.GetComponent<BoxCollider2D>().enabled = true;

		anim.SetTrigger ("Melee");
		audio.Play ();

		meleeTimer =  coolDownTimer;
	}
	void OnTriggerEnter2D (Collider2D col) 
	{
		// If it hits an enemy...
		if(col.tag == "Enemy"){
			
			// ... find the Enemy script and call the Hurt function.
			col.gameObject.GetComponent<Enemy>().Hurt();
			
			//Destroy(gameObject);
			Debug.Log ("Enemy hit");
		}
		// If melee atack hits a player...
		else if(col.gameObject.tag == "Player"){
			
			// ... find the StunBar script and call the TakeDamage function.
			col.gameObject.GetComponent<StunBar>().TakeDamage(10f);
		
			Debug.Log ("Player hit");
		}
		// ...else melee hits nothing
		else
			Debug.Log ("Hit nothing");
		// Disable BoxCollider2D
		this.GetComponent<BoxCollider2D>().enabled = false;

	}

}

﻿using UnityEngine;
using System.Collections;

public class Earthquake : Power {

	public float risetime;
	public GameObject groundshatter;
	// Use this for initialization
	void Start () {
		risetime = 0;
		Destroy(gameObject, 10);
	}
	
	// Update is called once per frame
	void Update () {
	
		if(risetime <= 0){

			transform.Translate(Vector3.down * 8 * Time.deltaTime);

		} else {


			transform.Translate (Vector3.up * 1 * Time.deltaTime);
			risetime -= Time.deltaTime;
		}

	}


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		if (GameObject.Find ("CollectData") != null) {
			GA.API.Design.NewEvent ("Earthquake Crushes", player.transform.position);
		}
		
		controller.Die ();

	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		
	}
	public override void PowerActionExit(GameObject player, Controller2D controller)
	{
		
	}

	[RPC]
	void QuakeParticle(Vector3 location, Quaternion rotate){

		Instantiate(groundshatter, location, rotate);

	}


	void OnTriggerEnter2D(Collider2D col)
	{

		//if (col.gameObject.CompareTag ("killplatform")) {
		if(col.transform.tag =="killplatform"){
			Instantiate(groundshatter, transform.position, transform.rotation);
			if(risetime <= 0){
				risetime = 2;
			}

			var players = GameObject.FindGameObjectsWithTag("Player");
			if(players.Length > 0){

				for(int i = 0; i < players.Length; i++){

					var currplayer = players[i];
					var networkscript = currplayer.GetComponent<NetworkController>();
					var playerstatus = currplayer.GetComponent<PlayerStatus>();
					var controller = currplayer.GetComponent<Controller2D>();
							
					if(networkscript.isOwner){
								
						if(controller.grounded){
							Instantiate(groundshatter, currplayer.transform.position, currplayer.transform.rotation);
							networkView.RPC("QuakeParticle", RPCMode.Others, currplayer.transform.position, currplayer.transform.rotation);
							if (GameObject.Find ("CollectData") != null) {
								GA.API.Design.NewEvent ("Earthquake Launches", currplayer.transform.position);
							}

							playerstatus.TakeDamage(100.0f);
							playerstatus.GenerateEvent(this);
							currplayer.rigidbody2D.AddForce(Vector3.up * 1500);
						}
					}
				}
			}
		}
	}


}

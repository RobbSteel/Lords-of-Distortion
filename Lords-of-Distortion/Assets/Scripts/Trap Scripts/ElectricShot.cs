﻿using UnityEngine;
using System.Collections;

public class ElectricShot : Power {

	private float speed = 3;
	public Vector3 direction;
	
	// Use this for initialization
	void Start () {
		Destroy (gameObject, 6.0f);
        particleSystem.renderer.sortingLayerName = "Foreground";
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Translate((direction) * (Time.deltaTime * speed)); 
		
	}


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{

		if (Analytics.Enabled) {
			GA.API.Design.NewEvent ("Electricity Kills", player.transform.position);
		}


        controller.Die(DeathType.FIRE);
	}
	
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}
	
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}




}

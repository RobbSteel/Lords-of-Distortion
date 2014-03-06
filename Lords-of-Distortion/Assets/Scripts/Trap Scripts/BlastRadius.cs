﻿using UnityEngine;
using System.Collections;

public class BlastRadius : Power {


	public override void PowerActionEnter(GameObject player, Controller2D controller){


		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Explosion Death", transform.position);
		}



		controller.Die();
			
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller)
	{

		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Explosion Death", transform.position);
		}


		controller.Die();


		
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller)
	{
		//print("exited");
		//controller.hasbomb = false;
	}



}

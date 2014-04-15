﻿using UnityEngine;
using System.Collections;

public class PowerHook : Power {


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		if (GameObject.Find ("CollectData") != null) {
			GA.API.Design.NewEvent ("Empowered Hook Kills", player.transform.position);
		}

		controller.Die();
	}

	public override void PowerActionStay (GameObject player, Controller2D controller)
	{

	}

	public override void PowerActionExit (GameObject player, Controller2D controller)
	{

	}


}

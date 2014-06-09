using UnityEngine;
using System.Collections;

public class Firewall : Power {



	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		


		if(Analytics.Enabled){
			GA.API.Design.NewEvent("Firewall Death", player.transform.position);
		}
		
		controller.Die(DeathType.FIRE);
	}


	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		controller.Die(DeathType.FIRE);
	}
	
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}
}

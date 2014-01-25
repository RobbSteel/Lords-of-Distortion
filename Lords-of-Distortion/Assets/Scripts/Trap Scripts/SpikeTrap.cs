using UnityEngine;
using System.Collections;

public class SpikeTrap : Power {

	public override void PowerAction(GameObject player, Controller2D controller){
		controller.Die();
	}
	
	public override void OnLoseContact (GameObject player, Controller2D controller)
	{
		//Nothing for now.
	}
}
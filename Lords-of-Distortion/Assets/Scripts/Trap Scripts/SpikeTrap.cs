using UnityEngine;
using System.Collections;

public class SpikeTrap : Power {

	public override void PowerActionEnter(GameObject player, Controller2D controller){
		controller.Die();
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){

	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){

	}
}
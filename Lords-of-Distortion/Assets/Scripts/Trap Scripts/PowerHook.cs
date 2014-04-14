using UnityEngine;
using System.Collections;

public class PowerHook : Power {


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		if(gameObject.GetComponent<HookHit>().shooter != player)
			controller.Die();
	}

	public override void PowerActionStay (GameObject player, Controller2D controller)
	{

	}

	public override void PowerActionExit (GameObject player, Controller2D controller)
	{

	}


}

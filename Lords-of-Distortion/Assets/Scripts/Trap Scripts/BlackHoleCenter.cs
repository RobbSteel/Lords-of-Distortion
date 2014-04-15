using UnityEngine;
using System.Collections;

public class BlackHoleCenter : Power 
{
	void Start () 
	{
		Destroy(gameObject, 15f);
	}
	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		controller.Die();
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{	
		controller.Die();
	}
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}

}

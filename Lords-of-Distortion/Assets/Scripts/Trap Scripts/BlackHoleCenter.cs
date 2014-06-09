using UnityEngine;
using System.Collections;

public class BlackHoleCenter : Power 
{
	void Start () 
	{
		//copy spawn info from parent.
		spawnInfo = new PowerSpawn(transform.parent.GetComponent<BlackHoleRadius>().spawnInfo);
		spawnInfo.type = PowerType.BH_INNER;
		Destroy(gameObject, 5f);
	}
	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		if (Analytics.Enabled) {
			GA.API.Design.NewEvent ("BlackHole Deaths", player.transform.position);
		}
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

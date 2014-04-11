using UnityEngine;
using System.Collections;

public class BlackHole : Power 
{
	public Transform target;
	bool entered = false;
	float speed = 5.0f;
	void Update(){



		if (entered){
			transform.position = Vector3.MoveTowards (transform.position, target.position, speed*Time.deltaTime);
		}

	}
	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		entered = true;

		//while (player.transform.position != transform.position) {
				
		//}
		//transform.LookAt(target);
		//controller.rigidbody2D.AddForce(transform.forward * 50);
		//player.rigidbody2D.AddForce(transform.right * 50);
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		
	}
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
		
	}
}

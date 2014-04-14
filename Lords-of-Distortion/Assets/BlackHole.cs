using UnityEngine;
using System.Collections;

public class BlackHole : Power 
{
	//Transform blackhole;
	bool entered = false;
	Transform target;
	GameObject player;
	Controller2D controller;
	void Update(){
	}

	void FixedUpdate(){
		float speed = 0.25f;
		//speed = speed / 4;

		if (entered) {
			Debug.Log ("movetoblack");
			moveT2BH(speed);
		}

	}

	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		entered = true;
		
		//controller.AddForce (0, 10, 0);
		//

		//player.transform.position = transform.position;

				
		//}
		//transform.LookAt(target);
		//controller.rigidbody2D.AddForce(transform.forward * 50);
		//player.rigidbody2D.AddForce(transform.right * 50);
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		entered = true;
		Debug.Log ("entered");

	}
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
		entered = false;
	}

	//Moves the player to hooked position and deletes links as they player comes into contact with them
	void move2BH(float speed){
		//dGameObject.FindGameObjectWithTag("Player").transform.rigidbody2D.velocity = Vector2.zero;
		//GameObject.FindGameObjectWithTag("Player").transform.rigidbody2D.gravityScale = 0;
		var distance = Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
		Debug.Log (distance);
		if (distance > 1) {
			GameObject.FindGameObjectWithTag ("Player").transform.position = Vector2.MoveTowards (GameObject.FindGameObjectWithTag ("Player").transform.position, transform.position, speed/distance);
		} else 
			entered = false;
		//	DestroyHookPossible();
			//print(distance);
		//}
	}

	//void OnCollisionStay2D(Collision2D col){
	//	if (col.gameObject.tag == "Player")
	//		controller.Die ();
	//}
}

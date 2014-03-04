using UnityEngine;
using System.Collections;

public class Boulder : Power {

	float direction = -1.0f;
	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		GA.API.Design.NewEvent("Boulder Death", player.transform.position);
		controller.Die();
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{

	}
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{

	}

	Vector2 force = new Vector2(-200f, 50);
	Vector2 velocity = new Vector2(8f, 0f); 

	// Use this for initialization
	void Start () {

		//move boulder either right or left.
		if(spawnInfo.direction.x > 0f){
			direction = 1.0f;
		}

		force.x = force.x * direction;
		velocity.x = velocity.x * direction;
	}


	// Update is called once per frame
	void Update () {
		//rigidbody2D.AddForce(force);
		rigidbody2D.velocity = new Vector2(velocity.x, rigidbody2D.velocity.y);
	}
	
}

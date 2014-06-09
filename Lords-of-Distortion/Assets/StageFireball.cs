using UnityEngine;
using System.Collections;

public class StageFireball : Power {

	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		controller.Die(DeathType.FIRE);
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		controller.Die(DeathType.FIRE);
	}
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{

	}



	// Use this for initialization
	void Start () {
		spawnInfo = new PowerSpawn();
		spawnInfo.type = PowerType.FIREBALL;
		Destroy(gameObject, 4f);
		rigidbody2D.AddForce(Vector2.up * 1200f);
	}
	
	// Update is called once per frame
	float previousY = 100f;
	void Update () {

		if(rigidbody2D.velocity.y < 0f && previousY >= 0f){
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}

		previousY = rigidbody2D.velocity.y;
	}
}

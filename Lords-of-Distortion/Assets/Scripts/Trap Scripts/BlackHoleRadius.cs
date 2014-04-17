using UnityEngine;
using System.Collections;

public class BlackHoleRadius : Power 
{
	//Transform blackhole;
	bool entered = false;
	Transform target;
	GameObject player;
	Controller2D controller;
	public float BHForce;
	void Start () 
	{
		Destroy(gameObject, 15f);
		BHForce = 0.175f;
	}
	void Update(){
	}
	
	void FixedUpdate(){
		if (entered) {
			move2BH(BHForce);
		}
	}
	
	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		entered = true;
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		entered = true;
		
	}
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
		entered = false;
	}
	
	//Moves the player to black hole center
	void move2BH(float speed){
		var distance = Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
		//Debug.Log (distance);
		if (distance > 0.1) {
			GameObject.FindGameObjectWithTag ("Player").transform.position = Vector2.MoveTowards (GameObject.FindGameObjectWithTag ("Player").transform.position, transform.position, BHForce/distance);
		} else 
			entered = false;
	}
}

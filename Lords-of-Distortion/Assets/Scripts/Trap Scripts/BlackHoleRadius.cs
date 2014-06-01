using UnityEngine;
using System.Collections;

public class BlackHoleRadius : Power 
{
	//Transform blackhole;
	bool entered = false;
	Transform target;
	GameObject playerGO;
	Controller2D controller;
	public float BHForce;
	public AudioClip bhsound;

	void Start () 
	{
		Destroy(gameObject, 8f);
		BHForce = 0.16f;
		audio.loop = true;
	}

	void FixedUpdate(){
		if (entered) {
            if(playerGO != null)
			    move2BH(BHForce);
		}
	}
	
	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
        playerGO = player;
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
		var distance = Vector2.Distance(playerGO.transform.position, transform.position);
		//Debug.Log (distance);
		if (distance > 0.1 && !playerGO.GetComponent<Controller2D>().dead && !playerGO.GetComponent<Controller2D>().powerInvulnerable) {
			playerGO.transform.position = Vector2.MoveTowards (playerGO.transform.position, transform.position, BHForce/distance);
		} else 
			entered = false;
	}
}

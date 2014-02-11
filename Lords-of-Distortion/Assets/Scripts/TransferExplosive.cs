using UnityEngine;
using System.Collections;

public class TransferExplosive : Power {
	
	public float timer = 8;
	public GameObject playerstuck;
	public bool stickready = true;
	public float stickytimer = 2;
	public bool firststick = true;
	
	public override void PowerActionEnter(GameObject player, Controller2D controller){
		
		print ("Entered bomb");
		
		//Check for the first collision of the bomb
		if(stickready && firststick){
			
			playerstuck = player;
			stickready = false;
			firststick = false;
			transform.rigidbody2D.velocity = Vector2.zero;
			collider2D.isTrigger = true;
			controller.hasbomb = true;
			
			//Check for any future transitions of the bomb
		} else if(stickready && !controller.hasbomb) {
			print("getting to here");
			playerstuck = player;
			controller.hasbomb = true;
			stickready = false;
			
		}
		
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller)
	{
		print ("give it up");
		
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller)
	{
		//controller.hasbomb = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Blow up if the time is out
		if(timer <= 0){
			
			print("Boom");
			
		} else {
			
			//Do this only if the bomb cannot be transfered
			if(!stickready){
				
				stickytimer -= Time.deltaTime;
				
				if(stickytimer <= 0){
					
					stickytimer = 2;
					stickready = true;
				}
			}
			
			//If a player is stuck, follow them
			if(playerstuck != null){
				
				transform.position = Vector2.MoveTowards(transform.position, playerstuck.transform.position, 5);
				
			}
			
			timer -= Time.deltaTime;
			
		}
	}
}	
	

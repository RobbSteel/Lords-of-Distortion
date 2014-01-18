using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {

	public  bool going = false;
	public bool movingtowards = false;
	public bool movingback = false;
	public bool hookinput = false;

	public Vector2 mousePos = new Vector2(0,0);
	public HookHit hookscript;

	private GameObject go;
	public  GameObject hook;

	
	public float hooktimer = 0;
	public float chainspawner = 1;
	public float pushpull = 0;







	void Start () {
	
	
	}
	

	void FixedUpdate () {
	
		float speed = 1.0f;
		speed = speed / 4;





		//If hook has hit something, initialize moving towards, otherwise, move the hook back to player
		if(going == true){
			if(hookscript.hooked == true){
				movingtowards = true;
				going = false;
			 
			}else if(hookscript.destroyed == true){
				movingback = true;
				going = false;
			} else if(hookscript.playerhooked == true){

				hookinput = true;
				going = false;

			}
		}

		//Get input from user and set cooldown to avoid repeated use.
		if(hooktimer <= 0){

			if (Input.GetMouseButtonDown(0)){
				mousePos = Input.mousePosition;
				mousePos = Camera.main.ScreenToWorldPoint(mousePos);
				go  = (GameObject)Instantiate(hook, transform.position, transform.rotation);
				going = true;
				hookscript = go.GetComponent<HookHit>();
				hookscript.shooter = gameObject;
				hooktimer = 5;
				print (hookscript.shooter);
				transform.rigidbody2D.gravityScale = 0;


			}

		} else {
			hooktimer -= Time.deltaTime;
		}


		if(going == true){

			hookgoing(speed);
		}

		if(hookinput == true){



			if(Input.GetMouseButtonDown(0) && pushpull == 0){

				pushpull = 1;

			}

			if(Input.GetMouseButtonDown(1) && pushpull == 0){

				pushpull = 2;
			}

			if(pushpull == 1){

				pullingplayer(speed);


			} else if(pushpull == 2){

				goingtoplayer(speed);


			}

		
		}



		if(movingtowards == true){
			movingtohook(speed);
		}


		if(movingback == true){
			hookmovingback(speed);
		}
}



	//Player pulling opponent to himself
	void pullingplayer(float speed){

	
			
			var player = hookscript.players;
			player.transform.position = Vector2.MoveTowards(player.transform.position, transform.position, speed);
			var distance = Vector2.Distance(player.transform.position, transform.position);
			
			if(distance < .5){
				
				Destroy(go);
				hookinput = false;
				pushpull = 0;
				
				transform.rigidbody2D.gravityScale = 1;
		}
			
			
			



	}

	//Player pulling himself to opponent
	void goingtoplayer(float speed){


			
			var player = hookscript.players;
			transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed);
			
			
			var distance = Vector2.Distance(go.transform.position, transform.position);
			
			if(distance < .5){
				
				Destroy (go);
				hookinput = false;
				pushpull = 0;
				
				transform.rigidbody2D.gravityScale = 1;
		}
			
			



	}


	//Instantiates links while the hook is traveling
	void hookgoing(float speed){

		go.transform.position = Vector2.MoveTowards(go.transform.position, mousePos, speed);
	
	}

	//Moves the player to hooked position and deletes links as they player comes into contact with them
	void movingtohook(float speed){

		var distance = Vector2.Distance(transform.position, go.transform.position);


			
			if(distance > .5){
				transform.position = Vector2.MoveTowards(transform.position, go.transform.position, speed);
			} else {
				Destroy(go);
			transform.rigidbody2D.gravityScale = 1;


				movingtowards = false;
				print(distance);
			}
	}

	//Moves the hook back to the player and deletes links as the hook comes into contact with them.
	void hookmovingback(float speed){

		var distance = Vector2.Distance(transform.position, go.transform.position);


			if(distance > .5){
				go.transform.position = Vector2.MoveTowards(go.transform.position, transform.position, speed);
			
			} else {
				Destroy(go);
			transform.rigidbody2D.gravityScale = 1;
			
				movingback = false;
				print(distance);
			}
	}



	
}

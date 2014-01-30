using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {

	public  bool going = false;
	public bool movingtowards = false;
	public bool movingback = false;
	public bool hookinput = false;

	public Vector3 mousePos = new Vector3(0,0, 0);
	public HookHit hookscript;

	private GameObject go;
	public  GameObject hook;

	
	public float hooktimer = 0;
	public float chainspawner = 1;
	public float pushpull = 0;
	public float decisiontimer = 0;





	NetworkController networkController;

	void Start () {
		networkController = GetComponent<NetworkController>();
	
	}
	
	void ShootHookLocal(Vector3 target){
		mousePos = target;
		go  = (GameObject)Instantiate(hook, transform.position, transform.rotation);
		going = true;
		hookscript = go.GetComponent<HookHit>();
		hookscript.networkController = networkController;
		hookscript.shooter = gameObject;
		hooktimer = 5;
		print (hookscript.shooter);

	}

	[RPC]
	void NotifyShootHook(Vector3 target){
		networkView.RPC("ShootHookSimulate", RPCMode.Others, target);
		ShootHookLocal(target);
	}

	[RPC]
	void ShootHookSimulate(Vector3 target){
		if(networkController.isOwner)
			return;
		ShootHookLocal(target);
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
				decisiontimer = 2;
				hookinput = true;
				going = false;

			}
		}

		//Get input from user and set cooldown to avoid repeated use.
		if(hooktimer <= 0){

			if (Input.GetMouseButtonDown(0) && networkController.isOwner){
				Vector3 mouseClick = Input.mousePosition;
				mouseClick = Camera.main.ScreenToWorldPoint(mouseClick);

				if(Network.isServer)
					networkView.RPC("ShootHookSimulate", RPCMode.Others, mouseClick);

				else
					networkView.RPC("NotifyShootHook", RPCMode.Server,mouseClick);
				ShootHookLocal(mouseClick);
			}

		} else {
			hooktimer -= Time.deltaTime;
		}


		if(going == true){
			hookgoing(speed);
		}

		if(hookinput == true){
			HookChoice(speed);
		}
		
		if(movingtowards == true){
			movingtohook(speed);
		}


		if(movingback == true){
			hookmovingback(speed);
		}
}


//Gives players the option to hook players to them or pull themselves to the hooked player.
	void HookChoice(float speed){

		
		if(Input.GetMouseButtonDown(0) && pushpull == 0){
			
			pushpull = 1;
			
		}
		
		if(Input.GetMouseButtonDown(1) && pushpull == 0){
			
			pushpull = 2;
		}
		
		if(decisiontimer > 0){
			decisiontimer -= Time.deltaTime;
			if(pushpull == 1){
				pullingplayer(speed);
			} else if(pushpull == 2){
				goingtoplayer(speed);
			}
		} else if(pushpull == 0){
				Destroy(go);
				hookinput = false;
		}
	}


	//Player pulling opponent to himself
	void pullingplayer(float speed){

	
			
			var player = hookscript.players;
			player.transform.position = Vector3.MoveTowards(player.transform.position, transform.position, speed);
			var distance = Vector3.Distance(player.transform.position, transform.position);
			
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
		transform.rigidbody2D.gravityScale = 0;
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

using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {

	public bool going = false;
	public bool movingtowards = false;
	public bool movingback = false;
	public bool hookpull = false;
	public bool hookthrown = false;

	public Vector3 mousePos = new Vector3(0,0, 0);
	public HookHit hookscript;

	private GameObject go;
	public  GameObject hook;
	public bool OFFLINE;
	
	public float hooktimer = 0;
	public float chainspawner = 1;
	public float pushpull = 0;
	public float hittimer = 0;
	public bool hookDisable;
    private float currentDrag;
	private Animator animator;

	NetworkController networkController;
	Controller2D  controller2D;

	void Start () {
		networkController = GetComponent<NetworkController>();
		controller2D = GetComponent<Controller2D>();
		animator = GetComponent<Animator>();
	}
	
	void ShootHookLocal(Vector3 target){


		go  = (GameObject)Instantiate(hook, transform.position, transform.rotation);
		going = true;
		hookscript = go.GetComponent<HookHit>();
		hookscript.networkController = networkController;
		hookscript.shooter = gameObject;
		//hooktimer = 1.5f;
		//Calculate angle from player to mouse and rotate hook that way.
		Vector3 direction = Vector3.Normalize(target - transform.position);
		mousePos = transform.position + 100f * direction;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

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

	void Update(){

        if(hooktimer > 0)
        {
            hooktimer -= Time.deltaTime;
        }
		//If hook has hit something, initialize moving towards, otherwise, move the hook back to player
		if(going == true){
			if(hookscript.hooked == true){
				hittimer = 1.5f;
				movingtowards = true;
				going = false;
				
			}else if(hookscript.destroyed == true){
				movingback = true;
				going = false;
			} else if(hookscript.playerhooked == true){
                hooktimer = 1.5f;
				//apparently clients cant rpc each other directly, so let server tell the character
				//on the hooked players client when to start pulling the hooked player.
				if(Network.isServer && !hookpull){
					hittimer = 2;
					pushpull = 1;
					going = false;
					hookpull = true;
					//If the server got hit it doesnt need to send this.
					if(!OFFLINE){
						if(hitPlayer != Network.player)
							networkView.RPC ("PullPlayer", hitPlayer); 
					}

				}
			}
		}

	}

	void FixedUpdate () {
	
		float speed = 1.0f;
		speed = speed / 4;
        currentDrag = controller2D.rigidbody2D.drag;
		//Get input from user and set cooldown to avoid repeated use.
        //previously hooktimer <= 0
		if(!hookthrown){
			if (Input.GetMouseButtonDown(1) && networkController.isOwner && !controller2D.snared && !controller2D.locked && hooktimer <= 0 && !hookDisable)
            {
                animator.SetFloat("Speed", 0);
                Vector3 mouseClick = Input.mousePosition;
				mouseClick = Camera.main.ScreenToWorldPoint(mouseClick);
				hookthrown = true;
				AudioSource.PlayClipAtPoint( controller2D.hookSfx , transform.position );
                
				if(!OFFLINE)
                {
					if(GameObject.Find("CollectData") != null){
						GA.API.Design.NewEvent("Hook Shot", mouseClick);
					}

					if(Network.isServer)
						networkView.RPC("ShootHookSimulate", RPCMode.Others, mouseClick);
					
					else
						networkView.RPC("NotifyShootHook", RPCMode.Server,mouseClick);
				}
                
                ShootHookLocal(mouseClick);
			}

		} /*else {
			hooktimer -= Time.deltaTime;
		}*/

		if(going == true){
			hookgoing(speed);
		}


		//Pull the player to us but destroy the hook after a fixed amount of time.
		if(hookpull == true){
		
			if(hittimer > 0){

				pullingplayer(speed);
			
			} else {

			DestroyHookPossible();
			
			}

			hittimer -= Time.deltaTime; 
		}
		

		if(movingtowards == true){

			if(hittimer > 0){
			
			movingtohook(speed);
			
			} else {

			DestroyHookPossible();

			}

			hittimer -= Time.deltaTime;
		}

		if(movingback == true){
			hookmovingback(speed);
		}
}


//Gives players the option to hook players to them or pull themselves to the hooked player.

	/*
	 * When player A chooses to pull player in, send RPC to the hook on player B that sets choice to 1.
	 */
	[RPC]
	void PullPlayer(){
		print ("pull");
		pushpull = 1;
		//slightly longer to account for lag
		hittimer = 2.2f;
		going = false;
		hookpull = true;
	}
	
	[RPC]
	void GoToPlayer(){
		pushpull = 2;
	}

	[RPC]
	void NotifyDestroyHook(){
		DestroyHook(); //actually destroy the hook now that you have permission
	}

	private void DestroyHook(){
		transform.rigidbody2D.gravityScale = 1;
		hookpull = false;
		pushpull = 0;
		hookthrown = false;
		movingtowards = false;
		movingback = false;
		going = false;
		if(hookscript != null){
			if(hookscript.affectedPlayerC2D != null){
				hookscript.affectedPlayerC2D.UnHooked();
				hookscript.affectedPlayerC2D = null;
			}
			hookscript.playerhooked = false;
		}
		if(go != null){
			Destroy(go);
		}

	}


	public void DestroyHookPossible(){
		if(!OFFLINE){
			if(Network.isServer){
				networkView.RPC ("NotifyDestroyHook", RPCMode.Others);
				DestroyHook();
			}
		}
		else {
			DestroyHook();
		}

	}

	//Player pulling opponent to himself
	void pullingplayer(float speed){

		var player = hookscript.hookedPlayer;
		///player died
		if(player == null){
			DestroyHookPossible();
			return;
		}
		//Because you don't have authority over the other player's position, only do this on the hooked player's client.
		if(!networkController.isOwner){
			player.transform.position = Vector3.MoveTowards(player.transform.position, transform.position, speed);
		}

		var distance = Vector3.Distance(player.transform.position, transform.position);
		//This needs to be put somehwere else, but for now it'll do.
		if(distance < .1f){
			if(!networkController.isOwner){
				hookscript.affectedPlayerC2D.FreeFromSnare();
			}
			DestroyHookPossible();
		}
	}

	NetworkPlayer hitPlayer;
	[RPC]
	void HitPlayer( Vector3 playerLocation, NetworkPlayer hitPlayer, NetworkMessageInfo info){
		//TODO: make this animation appear on all players screens
		hookscript.animator.SetTrigger("Hooked");
		print ("Hit by "  + info.networkView.viewID);
		print ("My id is " + networkView.viewID);
		go.transform.position = playerLocation;
		hookscript.hookedPlayer = SessionManager.Instance.psInfo.GetPlayerGameObject(hitPlayer);
		hookscript.affectedPlayerC2D = hookscript.hookedPlayer.GetComponent<Controller2D>();
		targetLocation = playerLocation;
		hookscript.targetPosition = playerLocation;
		hookscript.playerhooked = true;
		this.hitPlayer = hitPlayer;
	}

	//to be called by hookhit
	public void HitPlayerLocal(Vector3 playerLocation, NetworkPlayer hitPlayer){
		targetLocation = playerLocation;
		this.hitPlayer = hitPlayer;
	}

	Vector3 targetLocation;
	//Player pulling himself to opponent
	void goingtoplayer(float speed){
		var player = hookscript.hookedPlayer;
		transform.position = Vector2.MoveTowards(transform.position, hookscript.targetPosition, speed);
		var distance = Vector2.Distance(go.transform.position, transform.position);
		if(distance < .5){
			if(!networkController.isOwner){
				hookscript.affectedPlayerC2D.FreeFromSnare();
			}
			DestroyHookPossible();
		}
	}


	//Instantiates links while the hook is traveling
	void hookgoing(float speed){

		go.transform.position = Vector2.MoveTowards(go.transform.position, mousePos, speed);
	
	}

	//Moves the player to hooked position and deletes links as they player comes into contact with them
	void movingtohook(float speed){
		transform.rigidbody2D.velocity = Vector2.zero;
		transform.rigidbody2D.gravityScale = 0;
		var distance = Vector2.Distance(transform.position, go.transform.position);

		if(distance > 1){
			transform.position = Vector2.MoveTowards(transform.position, go.transform.position, speed);
		} else {
			DestroyHookPossible();
			//print(distance);
		}
	}

	//Moves the hook back to the player and deletes links as the hook comes into contact with them.
	void hookmovingback(float speed){

		var distance = Vector2.Distance(transform.position, go.transform.position);
			if(distance > 1){
				go.transform.position = Vector2.MoveTowards(go.transform.position, transform.position, speed);
			
			} else {
			DestroyHookPossible();
			//print(distance);
		}
	}

	void OnDestroy(){
		if(go != null)
			Destroy (go);
	}

}

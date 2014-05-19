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
	const float PLAYER_RELEASE_DISTANCE = .8f;
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
		//mousePos = transform.position + 100f * direction;
		mousePos = target;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

	}

	//Called on server
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
	float hookSpeed = 0f;
	float speedRatio = 12f;
	void Update(){
		hookSpeed = Time.deltaTime * speedRatio;
        if(hooktimer > 0)
        {
            hooktimer -= Time.deltaTime;
        }
		//If hook has hit something, initialize moving towards, otherwise, move the hook back to player
		if(going == true)
		{
			if(hookscript.hooked == true){
				hittimer = 1.5f;
				movingtowards = true;
				going = false;
			}
			else if(hookscript.returning == true)
			{
				movingback = true;
				going = false;
			}
			else if(hookscript.playerhooked == true)
			{
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



		currentDrag = controller2D.rigidbody2D.drag;
		//Get input from user and set cooldown to avoid repeated use.
		//previously hooktimer <= 0
		if(!hookthrown){
			if (Input.GetMouseButtonDown(1) && networkController.isOwner && !controller2D.snared && !controller2D.locked && hooktimer <= 0 && !hookDisable && !controller2D.crouching )
			{
				animator.SetFloat("Speed", 0);
				Vector3 mouseClick = Input.mousePosition;
				mouseClick = Camera.main.ScreenToWorldPoint(mouseClick);
				hookthrown = true;
				animator.SetTrigger("HookThrow");
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
		}

		if(going == true)
		{
			hookgoing(hookSpeed);
		}
		
		//Pull the player to us but destroy the hook after a fixed amount of time.
		if(hookpull == true)
		{
			if(hittimer > 0){
				pullingplayer(hookSpeed);
			} else {	
				DestroyHookPossible();
			}
			hittimer -= Time.deltaTime; 
		}
		
		
		if(movingtowards == true){
			
			if(hittimer > 0){
				movingtohook(hookSpeed);
				
			} else {
				DestroyHookPossible();
			}
			hittimer -= Time.deltaTime;
		}
		
		if(movingback == true){
			hookmovingback(hookSpeed);
		}
		
		
	}

	[RPC]
	void HookReturn(){
		hookscript.returning = true;
	}

	//Gives players the option to hook players to them or pull themselves to the hooked player.

	/*
	 * When player A chooses to pull player in, send RPC to the hook on player B that sets choice to 1.
	 */
	[RPC]
	void PullPlayer(){
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


	public void DestroyHookPossible(bool polite = false){
		if(!OFFLINE){
			/*
			if(Network.isServer){
				foreach(NetworkPlayer player in SessionManager.Instance.psInfo.players){
					if(player != networkController.theOwner && player != Network.player)
						networkView.RPC ("NotifyDestroyHook", player);
				}
				DestroyHook();
			}
			else{

			}
			 * */
			if(networkController.isOwner){
				networkView.RPC ("NotifyDestroyHook", RPCMode.Others);
				DestroyHook();
			}
			else if(polite){
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

		var hookedPlayer = hookscript.hookedPlayer;
		///player died
		if(hookedPlayer == null){
			DestroyHookPossible();
			return;
		}

		float distance = Vector3.Distance(hookedPlayer.transform.position, transform.position);
		//Because you don't have authority over the other player's position, only do this on the hooked player's client.
		if(!networkController.isOwner && Network.player == hookedPlayer.GetComponent<NetworkController>().theOwner){
			if(distance > PLAYER_RELEASE_DISTANCE)
				hookedPlayer.transform.position = Vector3.MoveTowards(hookedPlayer.transform.position, transform.position, hookSpeed);
			else{
				//we're free. tell everyone else too
				hookscript.affectedPlayerC2D.FreeFromSnare();
				DestroyHookPossible(true);
			}
		}
		//we dont want to wait for the hooked player to tell us to destroy this thing
		else if(networkController.isOwner && distance < PLAYER_RELEASE_DISTANCE)
		{
			DestroyHookPossible(true);
		}
	}

	NetworkPlayer hitPlayer;
	[RPC]
	void HitPlayer( Vector3 playerLocation, NetworkPlayer hitPlayer, NetworkMessageInfo info){
		//TODO: make this animation appear on all players screens
		hookscript.animator.SetTrigger("Hooked");
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
		transform.position = Vector2.MoveTowards(transform.position, hookscript.targetPosition, hookSpeed);
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
		go.transform.position = Vector2.MoveTowards(go.transform.position, mousePos, hookSpeed);
		float distance = Vector2.Distance(go.transform.position, mousePos);
		if(distance <= .05f){
			//Reached destination
			hookscript.returning = true;
		}
	}

	//Moves the player to hooked position and deletes links as they player comes into contact with them
	void movingtohook(float speed){
		if(networkController.isOwner){
			transform.rigidbody2D.velocity = Vector2.zero;
			transform.rigidbody2D.gravityScale = 0;
			var distance = Vector2.Distance(transform.position, go.transform.position);
			
			if(distance > .8f){
				transform.position = Vector2.MoveTowards(transform.position, go.transform.position, hookSpeed);
			} else {
				DestroyHookPossible();
			}
		}
	}

	//Moves the hook back to the player and deletes links as the hook comes into contact with them.
	void hookmovingback(float speed){
			var distance = Vector2.Distance(transform.position, go.transform.position);
				if(distance > 1f){
					go.transform.position = Vector2.MoveTowards(go.transform.position, transform.position, hookSpeed);
				
				} else {
					DestroyHookPossible();
			}
	}

	void OnDestroy(){
		if(go != null)
			Destroy (go);
	}
}

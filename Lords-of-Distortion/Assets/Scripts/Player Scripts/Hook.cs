using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {

	public Vector3 mousePos = new Vector3(0,0, 0);
	public HookHit hookscript;

	private GameObject go;
	public  GameObject hook;
	public bool OFFLINE;
	
	public float hooktimer = 0;
	public float chainspawner = 1;
	public float hittimer = 0;
	public bool hookDisable;
    private float currentDrag;
	private Animator animator;
	const float PLAYER_RELEASE_DISTANCE = .7f;
	NetworkController networkController;
	Controller2D  controller2D;

	void Start () {

		networkController = GetComponent<NetworkController>();
		controller2D = GetComponent<Controller2D>();
		animator = GetComponent<Animator>();
	}

	public bool HitSomething{
		get {
			return currentState == HookState.PullingPlayer || currentState == HookState.PullingSelf;
		}
	}

	public enum HookState{
		PullingSelf,
		PullingPlayer,
		GoingOut,
		GoingBack,
		None
	}

	public HookState currentState = HookState.None;
	void ShootHookLocal(Vector3 target){
		go  = (GameObject)Instantiate(hook, transform.position, transform.rotation);
		currentState = HookState.GoingOut;
		hookscript = go.GetComponent<HookHit>();
		hookscript.networkController = networkController;
		hookscript.shooter = gameObject;
		//hooktimer = 1.5f;
		//Calculate angle from player to mouse and rotate hook that way.
		Vector3 difference = target - transform.position;
		Vector2 direction = new Vector2(difference.x, difference.y).normalized;

		//mousePos = transform.position + 100f * direction;
		mousePos = target;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		go.rigidbody2D.velocity = direction * speedRatio;

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

	void FixedUpdate(){
		if(currentState == HookState.GoingOut)
		{
			hookgoing(hookSpeed);
		}
	}
	public void HitPlatform(){
		hittimer = 1.5f;
		currentState = HookState.PullingSelf;
	}

	public void ReturnHook(){
		Vector3 difference = transform.position - go.transform.position;
		Vector2 direction = new Vector2(difference.x, difference.y).normalized;
		Vector2 velocity = direction * speedRatio;
		go.rigidbody2D.velocity = velocity;
		currentState = HookState.GoingBack;
	}

	void Update(){
		hookSpeed = Time.deltaTime * speedRatio;


        if(hooktimer > 0)
        {
            hooktimer -= Time.deltaTime;
        }

		//If hook has hit something, initialize moving towards, otherwise, move the hook back to player
		if(currentState == HookState.GoingOut)
		{
			if(hookscript.playerhooked == true)
			{
                hooktimer = 1.5f;
				if(Network.isServer){
					hittimer = 2f;
					currentState = HookState.PullingPlayer;

					if(!OFFLINE){
						//Tell clone of this player on hooked players screen to start pulling him.
						if(hitPlayer != Network.player)
							networkView.RPC ("PullPlayer", hitPlayer);
						//tell actual owner of this player that he's hooking someoene
						if(networkView.owner != Network.player)
							networkView.RPC("PullPlayer", networkView.owner);
					}
				}
			}
		}



		currentDrag = controller2D.rigidbody2D.drag;
		//Get input from user and set cooldown to avoid repeated use.
		if(currentState == HookState.None){
			if (Input.GetMouseButtonDown(1) && networkController.isOwner && !controller2D.snared && !controller2D.locked && hooktimer <= 0 && !hookDisable && !controller2D.crouching )
			{
				animator.SetFloat("Speed", 0);
				Vector3 mouseClick = Input.mousePosition;
				mouseClick = Camera.main.ScreenToWorldPoint(mouseClick);
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


		
		//Pull the player to us but destroy the hook after a fixed amount of time.
		if(currentState == HookState.PullingPlayer)
		{
			if(hittimer > 0){
				pullingplayer(hookSpeed);
			} else {	
				DestroyHookPossible();
			}
			hittimer -= Time.deltaTime; 
		}
		
		
		if(currentState == HookState.PullingSelf){
			
			if(hittimer > 0){
				movingtohook(hookSpeed);
				
			} else {
				DestroyHookPossible();
			}
			hittimer -= Time.deltaTime;
		}
		
		if(currentState == HookState.GoingBack){
			hookmovingback(hookSpeed);
		}
	}
	//Gives players the option to hook players to them or pull themselves to the hooked player.
	
	[RPC]
	void PullPlayer(){
		hittimer = 2f;
		currentState = HookState.PullingPlayer;
	}

	[RPC]
	void NotifyDestroyHook(){
		DestroyHook(); //actually destroy the hook now that you have permission
	}

	private void DestroyHook(){
		transform.rigidbody2D.gravityScale = 1;
		currentState = HookState.None;
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
			if(networkController.isOwner || Network.isServer){
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
		var playercontrol = hookedPlayer.GetComponent<Controller2D>();
		///player died
		if(playercontrol.dead){
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
		go.transform.position = playerLocation;
		hookscript.hookedPlayer = SessionManager.Instance.psInfo.GetPlayerGameObject(hitPlayer);
		hookscript.affectedPlayerC2D = hookscript.hookedPlayer.GetComponent<Controller2D>();
		targetLocation = playerLocation;
		hookscript.targetPosition = playerLocation;
		hookscript.playerhooked = true;
		this.hitPlayer = hitPlayer;
		//TODO: make this animation appear on all players screens
		hookscript.animator.SetTrigger("Hooked");
	}

	//to be called by hookhit
	public void HitPlayerLocal(Vector3 playerLocation, NetworkPlayer hitPlayer){
		targetLocation = playerLocation;
		this.hitPlayer = hitPlayer;
	}

	Vector3 targetLocation;
	//Instantiates links while the hook is traveling
	void hookgoing(float speed){
		//go.transform.position = Vector2.MoveTowards(go.transform.position, mousePos, hookSpeed);
		float distance = Vector2.Distance(go.transform.position, mousePos);
		if(distance <= .25f){
			//Reached destination
			hookscript.returning = true;
			ReturnHook();
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
		float distance = Vector2.Distance(transform.position, go.transform.position);
		if(distance <= 1f){
			DestroyHookPossible();
		}
	}

	void OnDestroy(){
		if(go != null)
			Destroy (go);
	}
}

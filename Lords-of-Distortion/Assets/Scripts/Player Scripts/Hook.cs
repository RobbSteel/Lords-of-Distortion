using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {

	public Vector3 mousePos = new Vector3(0,0, 0);
	public HookHit currentHook;
	
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
		None,
		Hidden
	}

	public HookState currentState = HookState.None;


	//TODO: allow owner of hook to destroy it if he hooks a platform, synchornize this somehow.
	void ShootHookLocal(float originX, float originY, float targetX, float targetY){
		Vector3 origin = new Vector3(originX, originY, transform.position.z);
		GameObject go  = (GameObject)Instantiate(hook, origin, transform.rotation);
		currentState = HookState.GoingOut;
		currentHook = go.GetComponent<HookHit>();
		currentHook.networkController = networkController;
		currentHook.shooter = gameObject;
		//hooktimer = 1.5f;
		//Calculate angle from player to mouse and rotate hook that way.
		Vector3 target =  new Vector3(targetX, targetY, transform.position.z);
		Vector3 difference = target - origin;
		Vector2 direction = new Vector2(difference.x, difference.y).normalized;

		//mousePos = transform.position + 100f * direction;

		mousePos = target;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		go.rigidbody2D.velocity = direction * speedRatio;

	}

	//Called on server
	[RPC]
	void NotifyShootHook(float originX, float originY, float targetX, float targetY){
		networkView.RPC("ShootHookSimulate", RPCMode.Others,  originX, originY, targetX, targetY);
		ShootHookLocal(originX, originY, targetX, targetY);
	}

	[RPC]
	void ShootHookSimulate(float originX, float originY, float targetX, float targetY){
		if(networkController.isOwner)
			return;
		ShootHookLocal(originX, originY, targetX, targetY);
	}

	float hookSpeed = 0f;
	float speedRatio = 12f;
	
	public void HitPlatform(){
		hittimer = 1.5f;
		currentState = HookState.PullingSelf;
	}

	public void ReturnHook(){
		currentState = HookState.GoingBack;
		/* Physics.
		Vector3 difference = transform.position - currentHook.gameObject.transform.position;
		Vector2 direction = new Vector2(difference.x, difference.y).normalized;
		Vector2 velocity = direction * speedRatio;
		*/
		currentHook.rigidbody2D.isKinematic = true;
		currentHook.rigidbody2D.velocity = Vector2.zero;
		currentHook.returning = true;
	}

	void Update(){
		hookSpeed = Time.deltaTime * speedRatio;


        if(hooktimer > 0)
        {
            hooktimer -= Time.deltaTime;
        }

		currentDrag = controller2D.rigidbody2D.drag;
		//Get input from user and set cooldown to avoid repeated use.
		if(currentState == HookState.None || currentState == HookState.Hidden){
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
						networkView.RPC("ShootHookSimulate", RPCMode.Others, transform.position.x, transform.position.y,  mouseClick.x, mouseClick.y);
					
					else
						networkView.RPC("NotifyShootHook", RPCMode.Server, transform.position.x, transform.position.y,  mouseClick.x, mouseClick.y);
				}
				
				ShootHookLocal(transform.position.x, transform.position.y,  mouseClick.x, mouseClick.y);
			}
		}

		if(currentState == HookState.GoingOut)
		{
			hookgoing(hookSpeed);
		}
		
		//Pull the player to us but destroy the hook after a fixed amount of time.
		if(currentState == HookState.PullingPlayer)
		{
			if(hittimer > 0){
				pullingplayer(hookSpeed);
			} else {	
				DestroyHookPossible(Authority.SERVER);
			}
			hittimer -= Time.deltaTime; 
		}
		
		
		if(currentState == HookState.PullingSelf){
			
			if(hittimer > 0){
				movingtohook(hookSpeed);
				
			} else {
				DestroyHookPossible(Authority.OWNER);
			}
			hittimer -= Time.deltaTime;
		}
		
		if(currentState == HookState.GoingBack){
			hookmovingback(hookSpeed);
		}
	}

	[RPC]
	void NotifyDestroyHook(){
		DestroyHook(); //actually destroy the hook now that you have permission
	}


	private void DestroyHookSoft()
	{
		transform.rigidbody2D.gravityScale = 1;
		currentHook.renderer.enabled = false;
		currentHook.lr.enabled = false;
		currentState = HookState.Hidden;
	}

	private void DestroyHook(){
		transform.rigidbody2D.gravityScale = 1;
		currentState = HookState.None;
		if(currentHook != null){
			if(currentHook.affectedPlayerC2D != null){
				currentHook.affectedPlayerC2D.UnHooked();
			}
			Destroy(currentHook.gameObject);
			currentHook = null;
		}
	}
	
	public enum Authority
	{
		SERVER, OWNER, OTHER
	}

	public void DestroyHookPossible(Authority authority, bool polite = false){
		if(!OFFLINE){
			if((authority == Authority.SERVER && Network.isServer)
			   || (authority == Authority.OWNER && networkController.isOwner)
			   || (authority == Authority.OTHER && polite))
			{
				networkView.RPC ("NotifyDestroyHook", RPCMode.Others);
				DestroyHook();
			}

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
		}
		else {
			DestroyHook();
		}
	}

	//Player pulling opponent to himself
	void pullingplayer(float speed){

		GameObject hookedPlayer = currentHook.hookedPlayer;

		///player died or dcd
		if(hookedPlayer == null){
			DestroyHookPossible(Authority.SERVER);
			return;
		}
	
		Controller2D playercontrol = hookedPlayer.GetComponent<Controller2D>();

		if(playercontrol.dead)
		{
			DestroyHookPossible(Authority.SERVER);
			return;
		}
	
		float distance = Vector3.Distance(hookedPlayer.transform.position, transform.position);

		if(distance < PLAYER_RELEASE_DISTANCE && Network.isServer)
		{
			DestroyHookPossible(Authority.SERVER);
		}

		//Because you don't have authority over the other player's position, only do this on the hooked player's client.
		else if(!networkController.isOwner && Network.player == hookedPlayer.GetComponent<NetworkController>().theOwner){
			if(distance > PLAYER_RELEASE_DISTANCE)
				hookedPlayer.transform.position = Vector3.MoveTowards(hookedPlayer.transform.position, transform.position, hookSpeed);
			else{
				//we're free. tell everyone else too
				currentHook.affectedPlayerC2D.UnHooked();
				if(!Network.isServer)
					DestroyHookSoft();
			}
		}
	}

	NetworkPlayer hitPlayer;
	[RPC]
	void HitPlayer(float playerLocationX, float playerLocationY, NetworkPlayer hitPlayer, NetworkMessageInfo info){

		if(currentHook == null){
			print ("too late");
			return;
		}
		if(currentState == HookState.Hidden)
		{
			currentHook.renderer.enabled = false;
			currentHook.lr.enabled = false;
		}

		currentState = HookState.PullingPlayer;
		this.hitPlayer = hitPlayer;
		Vector3 playerLocation = new Vector3(playerLocationX, playerLocationY, transform.position.z);
		currentHook.gameObject.transform.position = playerLocation;
		currentHook.hookedPlayer = SessionManager.Instance.psInfo.GetPlayerGameObject(hitPlayer);
		currentHook.affectedPlayerC2D = currentHook.hookedPlayer.GetComponent<Controller2D>();
		currentHook.affectedPlayerC2D.Hooked();
		currentHook.targetPosition = playerLocation;
		currentHook.playerhooked = true;

		currentHook.animator.SetTrigger("Hooked");
		hittimer = 2f;
		hooktimer = 1.5f;
	}

	//To be called by HookHit
	public void HitPlayerLocal(NetworkPlayer hitPlayer)
	{
		this.hitPlayer = hitPlayer;
		hooktimer = 1.5f;
		hittimer = 2f;
		currentState = HookState.PullingPlayer;
		if(!OFFLINE)
			networkView.RPC ("HitPlayer", RPCMode.Others, currentHook.transform.position.x, currentHook.transform.position.y, hitPlayer);
	}
	
	//Instantiates links while the hook is traveling
	void hookgoing(float speed){
		//go.transform.position = Vector2.MoveTowards(go.transform.position, mousePos, hookSpeed);
		float distance = Vector2.Distance(currentHook.gameObject.transform.position, mousePos);
		if(distance <= .25f){
			//Reached destination
			currentHook.returning = true;
			ReturnHook();
		}
	}

	//Moves the player to hooked position and deletes links as they player comes into contact with them
	void movingtohook(float speed){
		if(networkController.isOwner || Network.isServer){

			Vector3 difference = currentHook.gameObject.transform.position - transform.position;
			Vector2 direction = new Vector2(difference.x, difference.y).normalized;
			Vector2 velocity = direction * speedRatio;
			transform.rigidbody2D.velocity = velocity;
			transform.rigidbody2D.gravityScale = 0;

			float distance = Vector2.Distance(transform.position, currentHook.gameObject.transform.position);
			
			if(distance <= .8f){
				transform.rigidbody2D.velocity = Vector2.zero;
				DestroyHookPossible(Authority.OWNER);
			}
		}
	}

	//Moves the hook back to the player and deletes links as the hook comes into contact with them.
	void hookmovingback(float speed){
		float distance = Vector2.Distance(transform.position, currentHook.gameObject.transform.position);
		if(distance <= .8f){
			if(!Network.isServer)
				DestroyHookSoft();
			DestroyHookPossible(Authority.SERVER);
		}
		else {
			currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, transform.position, hookSpeed);
		}
	}

	void OnDestroy(){
		if(currentHook != null)
			Destroy (currentHook.gameObject);
	}
}

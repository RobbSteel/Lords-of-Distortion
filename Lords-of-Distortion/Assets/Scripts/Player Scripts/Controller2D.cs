using UnityEngine;
using System.Collections;
using InControl;

public enum DeathType{
	CRUSH,
	FIRE,
	PLAGUE,
	EXPLOSION,
	FROZEN,
	RIPPED
}

public class Controller2D : MonoBehaviour {
	[HideInInspector]
	public bool facingRight;
	[HideInInspector]
	public bool jumpRequested = false;

	public bool OFFLINE;
	public float maxSpeed = 10f;
	public Animator anim;

	public float move;
	public bool dead = false;
	public float Lives_LOCAL = 0;
	public bool grounded = false;
	public Transform groundCheck;
	private float groundRadius = 0.14f;
	public LayerMask groundLayer;
	const float jumpVelocity = 12.5f;
	public bool stunned;
    public bool meleeStunned;
	public bool snared = false;
	public bool hooked = false;
	public bool deathOnHit;
	public bool canJump;
	public bool crouching = false;
	public bool hasbomb;
    public bool locked;
	bool stoppedJump;
	public bool moveDisable;
	public bool crouchDisable;
	public bool inAir = true;
	public bool powerInvulnerable;
	public delegate void DieAction(Controller2D controller, DeathType deathType);
	public static event DieAction onDeath; 
	public delegate void SpawnAction(NetworkPlayer player, Controller2D controller);
	public static event SpawnAction onSpawn;
	public float respawntime = 2f;
	public bool recentspawn = false;
	public Vector3 respawnPoint;
	public float invulntime = 0;
	public GameObject DeathSpirit;
	public GameObject RespawnHeart;
	public GameObject invulnshield;
	public GameObject newshield;
	//Player Audio Clips  --
	public AudioClip hookSfx;
	public AudioClip meleeSfx;
	public AudioClip meleeHitSfx;
	public AudioClip deathSfx;
	public AudioClip jumpSfx;

    public PhysicsMaterial2D playerMaterial;
    public PhysicsMaterial2D slipperyMaterial;

    public BoxCollider2D boxCollider;
    public CircleCollider2D circleCollider;

	NetworkController networkController;
	PlayerStatus status;
	Hook myHook;
    float knockbackTimer = 0f;

    private float resetGravityTimer = 0f;

	public bool knockedBack = false;
	
	public void KnockBack(){
		knockedBack = true;
		snared = true;
        meleeStunned = true;
	}

	public void Snare(){
		snared = true;
	}

    public void LockMovement()
   	{
        locked = true;
    }

   	public void UnlockMovement()
   	{
        locked = false;
    }
    
	public void Hooked(NetworkPlayer player){

		rigidbody2D.velocity = Vector2.zero;
		hooked = true;
		rigidbody2D.gravityScale = 0;
		anim.SetFloat("Speed", 0);
		//TODO: anim.SetTrigger("Hooked"); 
		if(networkController.isOwner && !dead)
		{
			status.GenerateEvent(PowerType.HOOK, TimeManager.instance.time, player);
		}
	}

	public void UnHooked(){
		hooked = false;
		rigidbody2D.gravityScale = 1;
	}
	//private StunBar stunScript;

	public void FreeFromSnare(){
		snared = false;
	}

	void Awake(){
		//crouchDisable = false;
		powerInvulnerable = false;
		deathOnHit = false;
		stunned = false;
        meleeStunned = false;
		facingRight = true;
		hasbomb = false;
        
		myHook = GetComponent<Hook>();
	}

	// Update is called once per frame
	void Update () {
		if(dead){
			if(respawntime > 0){
				respawntime -= Time.deltaTime;
			} else {
				respawn();
			}
		}

		if(recentspawn){
			if(invulntime > 0){
				invulntime -= Time.deltaTime;
			} else {
				recentspawn = false;
				powerInvulnerable = false;
				Destroy(newshield);
			}
		}

		if(!OFFLINE && !networkController.isOwner)
			return;

		if(!hooked && !locked){

		    JumpInput();
			move = GetHorizontalInput();
			if(move != 0 && !snared)
			{
				boxCollider.sharedMaterial = slipperyMaterial;
				circleCollider.sharedMaterial = slipperyMaterial;
			}

			else 
			{
				boxCollider.sharedMaterial = playerMaterial;
				circleCollider.sharedMaterial = playerMaterial;
			}
			CrouchInput();

            if(snared)
            { 
                rigidbody2D.gravityScale = 1;
            }
            if(meleeStunned)
            {
                knockbackTimer += Time.deltaTime;
                if(knockbackTimer > 2)
                {
                    knockbackTimer = 0;
                    FreeFromSnare();
                }
            }
        }		
	}

	private void JumpInput(){
		if(!snared && !locked && !myHook.HitSomething && !stunned && grounded && canJump)
		{
			if(GameInput.instance.usingGamePad)
			{
				if(InputManager.ActiveDevice.Action1.WasPressed)
					jumpRequested = true;
			}
			else 
			{
				if(Input.GetKeyDown(KeyMapping.JumpKey))
					jumpRequested = true;
			}
		}
		if(inAir)
		{
			if(GameInput.instance.usingGamePad )
			{
				stoppedJump = !InputManager.ActiveDevice.Action1.IsPressed;
			}
			else 
			{
				stoppedJump = !Input.GetKey(KeyMapping.JumpKey);
			}
		}
	}

	private float GetHorizontalInput()
	{
		if(GameInput.instance.usingGamePad)
		{
			InputDevice device = InputManager.ActiveDevice;
			if(device.DPad.Left.IsPressed)
			{
				return -1f;
			}
			else if(device.DPad.Right.IsPressed)
			{
				return 1f;
			}
			else
			{
				return device.LeftStickX.Value;
			}
		}
		else
		{
			if(Input.GetKey(KeyMapping.LeftKey))
			{
				return -1f;
			}
			else if(Input.GetKey(KeyMapping.RightKey))
			{
				return 1f;
			}
			else
			{
				return 0f;
			}
		}
	}
	
	float previousY = 0f;
	void FixedUpdate(){

        if(rigidbody2D.gravityScale < 0)
        {
            resetGravityTimer += Time.deltaTime;
            if(resetGravityTimer >= 1)
            {
                rigidbody2D.gravityScale = 1;
                resetGravityTimer = 0;
            }
        }

        if(transform.parent != null && (myHook.HookOut || inAir || !myHook.HookOut ))
        {
            transform.parent = null;
        }

		if(!hooked){

            IsGrounded();

		    if(!OFFLINE && !networkController.isOwner)
			    return;
            

            if(!locked && !moveDisable )
		        MovePlayer();

		    //Increase gravity scale when jump is at its peak or when user lets go of jump button.
		    if((rigidbody2D.velocity.y < 0f && previousY >= 0f || stoppedJump)){
			    //print ("started falling");
			    rigidbody2D.gravityScale = 1.8f;
		    }

		    //Remove knockback when you compose yourself.
		    if(grounded && knockedBack && rigidbody2D.velocity.magnitude <= maxSpeed / 4f){
			    knockedBack = false;
			    FreeFromSnare();
		    }

		    // If player jumps
		    if (jumpRequested) {
			    // set the Jump animator trigger parameter
			    anim.SetTrigger("Jump");
			    //AudioSource.PlayClipAtPoint( jumpSfx , transform.position);
			    //I think setting velocity feels better.
				rigidbody2D.gravityScale = 1f;
			    rigidbody2D.velocity  = new Vector2(rigidbody2D.velocity.x, jumpVelocity);
			    inAir = true;
                boxCollider.sharedMaterial = slipperyMaterial;
                circleCollider.sharedMaterial = slipperyMaterial;
			    // player can't jump again until jump conditions from Update are satisfied
			    jumpRequested = false;
		    }
		    previousY = rigidbody2D.velocity.y;
		}
	}

	

	public void SetCrouchState(bool enabled)
	{
		canJump = !enabled;
		moveDisable = enabled;
		crouching = enabled;
		if(enabled)
			anim.SetTrigger("Crouch");
		else 
			anim.SetTrigger("Default");
	}

	//checks to see if our player can crouch and resets crouch once Crouch Input is released
	private void CrouchInput(){
		if(crouchDisable)
			return;
		if(!crouching)
		{
			if(!snared && !locked && !myHook.HitSomething && !stunned && grounded){
				if(GameInput.instance.usingGamePad)
				{
					if(InputManager.ActiveDevice.DPadDown.State ||
					   InputManager.ActiveDevice.LeftStick.Down.Value > .8f)
					{
						SetCrouchState(true);
					}
					
				}
				else if(Input.GetKey(KeyMapping.CrouchKey))
				{
					SetCrouchState(true);
				}
			}
		}
		else {
			if(GameInput.instance.usingGamePad)
			{
				if(InputManager.ActiveDevice.DPadDown.WasReleased ||
				   InputManager.ActiveDevice.LeftStick.Down.WasReleased)
				{
					SetCrouchState(false);
				}
			}
			else if(Input.GetKeyUp(KeyMapping.CrouchKey))
			{
				SetCrouchState(false);
			}
		}

	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		networkController = GetComponent<NetworkController>();
		status = GetComponent<PlayerStatus>();
		if(onSpawn != null)
		{
			onSpawn(networkController.theOwner, this);
		}
	}

	//Constantly checks if player is on the ground
	void IsGrounded(){
		grounded = Physics2D.OverlapCircle(groundCheck.position , groundRadius, groundLayer );

            anim.SetBool( "Ground", grounded );
		if(inAir && grounded){
			inAir = false;
			rigidbody2D.gravityScale = 1f;
            boxCollider.sharedMaterial = playerMaterial;
            circleCollider.sharedMaterial = null;
		}
	}

	//Needs to go in fixedUpdate since we use physics to move player.
	void MovePlayer(){
		if( !stunned && !snared && !myHook.HitSomething && !hooked){
			//anim.SetFloat ( "vSpeed" , rigidbody2D.velocity.y );
			
			//to make jumping and changing direction is disabled
			anim.SetFloat("Speed", Mathf.Abs(move));
			//Problem: THis sets velocity to zero.
			rigidbody2D.velocity = new Vector2( move * maxSpeed, rigidbody2D.velocity.y );
			if( move < 0 && facingRight ){
				Flip();
			}
			else if( move > 0 && !facingRight ){
				Flip();
			}
		}
	}

	//Flips player sprite accordingly to which side he is facing
	public void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}



	//checks for collisions on impact and apply's powers on player 
	void OnTriggerEnter2D(Collider2D other)
	{
		if(dead || !networkController.isOwner)
			return;

		if(!powerInvulnerable)
		{
			if (other.gameObject.tag == "Power")
			{
				Power power = other.gameObject.GetComponent<Power>();
				if(!OFFLINE){
					status.GenerateEvent(power);
				}
				power.PowerActionEnter(gameObject, this);
				
			}
			
			else if(other.gameObject.tag == "PowerHook"){
				//ignore our own hook.
				if(other.gameObject.GetComponent<HookHit>().shooter == gameObject)
					return;
				
				Power power = other.gameObject.GetComponent<Power>();
				if(!OFFLINE){
					status.GenerateEvent(power);
				}
				power.PowerActionEnter(gameObject, this);
			}
		}


        if (other.gameObject.tag == "movingPlatform")
        {
            if (myHook.currentState != Hook.HookState.None)
                transform.parent = null;
            else
                transform.parent = other.transform;
        }
        
	}

	//while player is within the powers collider apply's powers on player 
	void OnTriggerStay2D(Collider2D other)
    {
		if (dead || !networkController.isOwner)
            return;

        if (other.gameObject.tag == "movingPlatform")
        {
            if (myHook.currentState != Hook.HookState.None)
                transform.parent = null;
            else
                transform.parent = other.transform;
        }

		if (!powerInvulnerable && other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();
			power.PowerActionStay(gameObject, this);
		}
	}

	//when player exits the collider it apply's powers after effects on player
	void OnTriggerExit2D(Collider2D other)
	{
		if(dead || !networkController.isOwner)
			return;
		
		if (other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();
			power.PowerActionExit(gameObject, this);
		}

        if (other.gameObject.tag == "movingPlatform")
        {
            if (myHook.currentState != Hook.HookState.None)
                transform.parent = null;
            else
                transform.parent = other.transform;
        }
	}

	//while player is colliding with the powers collider apply's powers on player 
	void OnCollisionStay2D(Collision2D col ) {

		if(dead || !networkController.isOwner)
			return;
        if (!powerInvulnerable && col.gameObject.tag == "Power"){
			Power power = col.gameObject.GetComponent<Power>();
			power.PowerActionStay(gameObject, this);
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if(dead || !networkController.isOwner)
			return;
		if (!powerInvulnerable && other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();

			if(!OFFLINE)
				status.GenerateEvent(power);

			power.PowerActionEnter(gameObject, this);
		}
	}

    void OnCollisionExit2D(Collision2D other)
    {
		if (dead || !networkController.isOwner)
            return;
		
        if (!powerInvulnerable && other.gameObject.tag == "Power")
        {
            Power power = other.gameObject.GetComponent<Power>();
            power.PowerActionExit(gameObject, this);
        }
    }


	//Because it would be a hassle to pass arguments to an animationm, store option here.
	private bool wantRespawn = false;

	public void Die(DeathType deathType = DeathType.CRUSH){
		if(networkController.isOwner && !dead){
			status.currentStunMeter = 0;
			collider2D.enabled = false;
			dead = true;
			locked = true;
			myHook.DestroyHookPossible(Hook.Authority.SERVER);

			Lives_LOCAL--;

			//respawn if we have lives or are in lobby (changing either of these two
			//values in memory would allow for infinite respawns :o)
			wantRespawn = Lives_LOCAL > 0 || LobbyGUI.inLobby;

			//play death animation.
			DeathAnimation(deathType);
           
			if(onDeath != null)
				onDeath(this, deathType);

			if(!wantRespawn && !LobbyGUI.inLobby){
				GameObject.Find("UI-death").GetComponent<UISprite>().enabled = true;
	            GameObject.Find("UI-deathCD").GetComponent<UISprite>().enabled = true;
			}
		}
	}


	public void DieSimple(DeathType deathType, bool respawn = false){
		if(!networkController.isOwner && !dead){
			dead = true;
			collider2D.enabled = false;
			wantRespawn = respawn;
			myHook.DestroyHookPossible(Hook.Authority.SERVER);
			DeathAnimation(deathType);
		}
	}


	void DeathAnimation(DeathType deathType){
		switch(deathType){
		case DeathType.FIRE:
			anim.SetTrigger("FireDeath");
			break;
		case DeathType.PLAGUE:
			anim.SetTrigger("PlagueDeath");
			break;
		case DeathType.EXPLOSION:
			anim.SetTrigger ("ExplosionDeath");
			break;
		case DeathType.RIPPED:
			anim.SetTrigger ("RippedDeath");
			break;
        case DeathType.CRUSH:
            anim.SetTrigger("CrushedDeath");
            break;
		case DeathType.FROZEN:
			anim.SetTrigger("ShatterDeath");
			break;
		default:
			//audio.PlayOneShot(deathSfx);
			anim.SetTrigger("Die"); //not gonna work
			break;
		}
	}

	/*
	 * Sequence of events:
	 * Die or DieSimple is called
	 * Death animation triggers and finishes playing 
	 * Death spirit is instantiated
	 * Player object is destroyed
	 */


	void respawn(){
		respawntime = 2f;
		var renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.enabled = true;
		dead = false;
		locked = false;
        if(crouching)
			SetCrouchState(false);
		hasbomb = false;
		status.currentStunMeter = 0;
		collider2D.enabled = true;
		invulntime = 3;
		recentspawn = true;
		powerInvulnerable = true;
		status.RemovePlague();
		Instantiate(RespawnHeart, transform.position, transform.rotation);
		newshield = (GameObject)Instantiate(invulnshield, transform.position, transform.rotation);
		newshield.transform.parent = gameObject.transform;
	}

 	void loseLife(){
		transform.parent = null;
		var renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.enabled = false;
		transform.position = respawnPoint;
	}

	//Called when death animation finishes playing.
	public void DestroyPlayer()
	{
		Instantiate(DeathSpirit, transform.position, transform.rotation);
		status.currentStunMeter = 0;

		if(!LobbyGUI.inLobby){
			if(!OFFLINE){
				if(wantRespawn)
					loseLife();
				else
					Destroy(gameObject);
			}
			else {
				Destroy(gameObject);
			}
		} else {
			status.currentStunMeter = 0;
			loseLife();
		}
	}

	void OnDestroy(){
		if(!OFFLINE){
			if(Network.isServer){
				//blocks any lingering rpc calls
				//Network.RemoveRPCs(networkView.viewID);
			}
		}
	}

}

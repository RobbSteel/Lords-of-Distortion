using UnityEngine;
using System.Collections;
using InControl;

public enum DeathType{
	CRUSH,
	FIRE,
	PLAGUE,
	EXPLOSION,
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
	public bool grounded = false;
	public Transform groundCheck;
	private float groundRadius = 0.2f;
	public LayerMask groundLayer;
	public float jumpForce = 650f;
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
	public delegate void DieAction(GameObject gO, DeathType deathType, float lives);
	public static event DieAction onDeath; 
	public delegate void SpawnAction(NetworkPlayer player);
	public static event SpawnAction onSpawn;
	public float lives;
	public float respawntime = 3;
	public bool recentspawn = false;
	public Vector3 respawnpoint;
	public float invulntime = 0;
	public GameObject DeathSpirit;
	public GameObject Respawn;
	public GameObject invulnshield;
	public GameObject newshield;
	//Player Audio Clips  --
	public AudioClip hookSfx;
	public AudioClip meleeSfx;
	public AudioClip meleeHitSfx;
	public AudioClip deathSfx;
	public AudioClip jumpSfx;

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

		if(networkController.isOwner)
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
		if(!OFFLINE && GameObject.Find("LobbyGUI") == null){
			lives = GameObject.FindGameObjectWithTag("ArenaManager").GetComponent<ArenaManager>().totallives;
		}
		crouchDisable = true;
		powerInvulnerable = false;
		deathOnHit = false;
		stunned = false;
        meleeStunned = false;
		facingRight = true;
		hasbomb = false;
		myHook = GetComponent<Hook>();

		if(GameObject.Find ("Respawn") != null){

			respawnpoint = GameObject.Find("Respawn").transform.position;
		}
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

			Crouch();
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

		if(GameInput.instance.usingGamePad)
		{
			stoppedJump = !InputManager.ActiveDevice.Action1.IsPressed;
		}
		else 
		{
			stoppedJump = Input.GetKeyUp(KeyMapping.JumpKey);
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

		if(!hooked){

            IsGrounded();

		    if(!OFFLINE && !networkController.isOwner)
			    return;
            

            if(!locked && !moveDisable )
		        MovePlayer();

		    //Increase gravity scale when jump is at its peak or when user lets go of jump button.
		    if(rigidbody2D.velocity.y < 0f && previousY >= 0f || stoppedJump){
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
			    //Add a vertical force to player
			    //rigidbody2D.AddForce(new Vector2(0f, jumpForce));

			    //Incase you were walking down a slope, rest gravity before jumping
			    rigidbody2D.gravityScale = 1f;

			    //I think setting velocity feels better.
			    rigidbody2D.velocity  = new Vector2(rigidbody2D.velocity.x, jumpVelocity);
			    inAir = true;

			    // player can't jump again until jump conditions from Update are satisfied
			    jumpRequested = false;
		    }
		    previousY = rigidbody2D.velocity.y;

	
		}
}


	
	//checks to see if our player can crouch and resets crouch once Crouch Input is released
	private void Crouch(){
		if(!snared && !locked && !myHook.HitSomething && !stunned && grounded && Input.GetButtonDown("Crouch") && !crouchDisable ){
			moveDisable = true;

			canJump = false;
			crouching = true;
			anim.SetBool("Crouch", crouching );
		}
		if( Input.GetButtonUp("Crouch") && !crouchDisable ){
			crouching = false;
			canJump = true;
			moveDisable = false;
			anim.SetBool("Crouch", crouching );
		}
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		networkController = GetComponent<NetworkController>();
		status = GetComponent<PlayerStatus>();
		if(onSpawn != null)
		{
			onSpawn(networkController.theOwner);
		}
	}

	//Constantly checks if player is on the ground
	void IsGrounded(){
		grounded = Physics2D.OverlapCircle(groundCheck.position , groundRadius, groundLayer );

            anim.SetBool( "Ground", grounded );
		if(inAir && grounded){
			inAir = false;
			rigidbody2D.gravityScale = 1f;
		}
	}

	//Needs to go in fixedUpdate since we use physics to move player.
	void MovePlayer(){
		if( !stunned && !snared && !myHook.HitSomething && !hooked){
			//anim.SetFloat ( "vSpeed" , rigidbody2D.velocity.y );
			
			//to make jumping and changing direction is disabled
			//if(!grounded) return;
		    move = Input.GetAxis ( "Horizontal" );
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
		
		if (!powerInvulnerable && (other.gameObject.tag == "Power" || other.gameObject.tag == "PowerHook" ))
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
        if (dead)
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
		if(dead)
			return;
		
		if (other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();
			power.PowerActionExit(gameObject, this);
		}

        if (other.gameObject.tag == "movingPlatform")
        {
            transform.parent = null;
        }
	}

	//while player is colliding with the powers collider apply's powers on player 
	void OnCollisionStay2D(Collision2D col ) {
		if(dead)
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
        if (dead)
            return;
		
        if (!powerInvulnerable && other.gameObject.tag == "Power")
        {
            Power power = other.gameObject.GetComponent<Power>();
            power.PowerActionExit(gameObject, this);
        }
    }



	public void Die(DeathType deathType = DeathType.CRUSH){

		if(networkController.isOwner && !dead){

			status.currentStunMeter = 0;
			collider2D.enabled = false;
			dead = true;
			locked = true;

			myHook.DestroyHookPossible(Hook.Authority.SERVER);
			/*Upon Death, tell the DeadLord Script that the player is dead by setting
			the boolean to true*
			var deadlord = GameObject.Find("DeadLordsScreen");
			var deadlordscript = deadlord.GetComponent<DeadLord>();
			deadlordscript.isDead = true;
			*/

			//Here we call whatever events are subscribed to us.
			if(GameObject.Find("LobbyGUI") == null){
			//Remove MashIcon from PlayerStatus Script
			//status.DestroyMashIcon();
			lives--;
			}

			if(onDeath != null)
				onDeath(gameObject, deathType, lives);

			//play death animation.
			DeathAnimation(deathType);
           
			if(GameObject.Find("LobbyGUI") == null && lives == 0){
				GameObject.Find("UI-death").GetComponent<UISprite>().enabled = true;
	            GameObject.Find("UI-deathCD").GetComponent<UISprite>().enabled = true;
			}
			//We don't need the next line any more
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
		default:
			anim.SetTrigger("Die"); //not gonna work
			break;
		}
	}

    [RPC]
    void SendAnimation(string myDeath)
    {
        anim.SetTrigger(myDeath);
    }

	public void DieSimple(DeathType deathType, float lives = 0){
		if(!networkController.isOwner && !dead){


			dead = true;
			collider2D.enabled = false;
			DeathAnimation(deathType);
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
		respawntime = 3;
		var renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.enabled = true;
		dead = false;
		locked = false;
		hasbomb = false;
		status.currentStunMeter = 0;
		collider2D.enabled = true;
		invulntime = 3;
		recentspawn = true;
		powerInvulnerable = true;
		status.RemovePlague();

		Instantiate(Respawn, new Vector2(0,0), transform.rotation);
		newshield = (GameObject)Instantiate(invulnshield, transform.position, transform.rotation);
		newshield.transform.parent = gameObject.transform;
	}

 void loselife(){

		Instantiate(DeathSpirit, transform.position, transform.rotation);
		transform.parent = null;

		var renderer = gameObject.GetComponent<SpriteRenderer>();
		renderer.enabled = false;
		transform.position = respawnpoint;
	}

	[RPC]
	void DeadPlayer(){

		Destroy(gameObject);
	}

	//Called when death animation finishes playing.
	public void DestroyPlayer()
	{
		if(GameObject.Find("LobbyGUI") == null){
			Instantiate(DeathSpirit, transform.position, transform.rotation);
			status.currentStunMeter = 0;
			if(lives == 0){
			Destroy (gameObject);
				networkView.RPC("DeadPlayer", RPCMode.Others);
			} else{
				if(OFFLINE)
					Destroy(gameObject);
				else
					loselife();
			}

		} else {
			status.currentStunMeter = 0;
			loselife ();
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

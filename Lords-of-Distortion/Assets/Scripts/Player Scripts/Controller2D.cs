﻿using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour {
	[HideInInspector]
	public bool facingRight;
	[HideInInspector]
	public bool jumpRequested = false;

	public bool DEBUG;
	public float maxSpeed = 10f;
	public Animator anim;

	private bool dead = false;
	public bool grounded = false;
	public Transform groundCheck;
	private float groundRadius = 0.2f;
	public LayerMask groundLayer;
	public float jumpForce = 650f;
	const float jumpVelocity = 12.5f;
	public bool stunned;
	public bool snared = false;
	public bool canJump;
	public bool hasbomb;
	bool stoppedJump;
	public bool inAir = true;
	public delegate void DieAction(GameObject gO);
	public static event DieAction onDeath; 
	


	NetworkController networkController;
	PlayerStatus status;
	Hook myHook;

	public bool knockedBack = false;
	
	public void KnockBack(){
		knockedBack = true;
		snared = true;
	}

	public void Snare(){
		snared = true;
	}
	//private StunBar stunScript;

	public void FreeFromSnare(){
		snared = false;
	}

	void Awake(){
		stunned = false;
		canJump = true;
		facingRight = true;
		hasbomb = false;
		myHook = GetComponent<Hook>();
	}

	// Update is called once per frame
	void Update () {
		if(!DEBUG && !networkController.isOwner)
			return;
		Jump();
		stoppedJump = Input.GetButtonUp("Jump");
	}
	
	float previousY = 0f;
	void FixedUpdate(){
		IsGrounded();
		if(!DEBUG && !networkController.isOwner)
			return;

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

	private void Jump(){
		if(!snared &&  !stunned && grounded && !myHook.hookthrown && Input.GetButtonDown("Jump") && canJump){
			jumpRequested = true;
		}
	}
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		networkController = GetComponent<NetworkController>();
		status = GetComponent<PlayerStatus>();
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
		if( !stunned && !snared && !myHook.hookthrown){
			//anim.SetFloat ( "vSpeed" , rigidbody2D.velocity.y );
			
			//to make jumping and changing direction is disabled
			//if(!grounded) return;
			float move = Input.GetAxis ( "Horizontal" );
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
		
		if (other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();
			power.PowerActionEnter(gameObject, this);
		}
	}

	//while player is within the powers collider apply's powers on player 
	void OnTriggerStay2D(Collider2D other)
    {
        if (dead)
            return;

		if (other.gameObject.tag == "Power")
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
	}

	//while player is colliding with the powers collider apply's powers on player 
	void OnCollisionStay2D(Collision2D col ) {
		if(dead)
			return;
		
        if (col.gameObject.tag == "Power"){
			Power power = col.gameObject.GetComponent<Power>();
			power.PowerActionStay(gameObject, this);
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		//This should have been done earlier. Power effects shouldn't matter
		//if they hit a copy of a player you don't control.
		if(dead || !networkController.isOwner)
			return;
		
		if (other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();
			power.PowerActionEnter(gameObject, this);
		}
	}

    void OnCollisionExit2D(Collision2D other)
    {
        if (dead)
            return;
		
        if (other.gameObject.tag == "Power")
        {
            Power power = other.gameObject.GetComponent<Power>();
            power.PowerActionExit(gameObject, this);
        }
    }



	public void Die(){
		if(dead == false){

			dead = true;
			snared = true;

			/*Upon Death, tell the DeadLord Script that the player is dead by setting
			the boolean to true*
			var deadlord = GameObject.Find("DeadLordsScreen");
			var deadlordscript = deadlord.GetComponent<DeadLord>();
			deadlordscript.isDead = true;
			*/
			Debug.Log ("I died again");

			//Here we call whatever events are subscribed to us.
			if(onDeath != null)
				onDeath(gameObject);

			//play death animation.
			anim.SetTrigger("Die");
			//We don't need the next line any more
			//networkController.instanceManager.KillPlayer(gameObject);
		}
	}

	void OnDisable(){
		myHook.DestroyHookPossible();
	}
}

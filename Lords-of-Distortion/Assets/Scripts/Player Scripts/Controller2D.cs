﻿using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour {
	public bool DEBUG;
	public float maxSpeed = 10f;
	public bool facingRight = false;
	public Animator anim;

	private bool dead = false;
	private bool grounded = false;
	public Transform groundCheck;
	private float groundRadius = 0.2f;
	public LayerMask groundLayer;
	public float jumpForce = 700f;
	public bool stunned;
	public bool isAttacking;
	public bool canJump;

	public delegate void DieAction(GameObject gO);
	public static event DieAction onDeath; 

	NetworkController networkController;

	void Awake(){
		stunned = false;
		isAttacking = false;
		canJump = true;
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		networkController = GetComponent<NetworkController>();
	}

	//Constantly checks if player is on the ground
	void IsGrounded(){
		grounded = Physics2D.OverlapCircle(groundCheck.position , groundRadius, groundLayer );
		anim.SetBool( "Ground", grounded );
	}

	//Needs to go in fixedUpdate since we use physics to move player.
	void MovePlayer(){
		if( !stunned ){
			anim.SetFloat ( "vSpeed" , rigidbody2D.velocity.y );
			
			//to make jumping and changing direction is disabled
			//if(!grounded) return;
			
			float move = Input.GetAxis ( "Horizontal" );
			
			anim.SetFloat("Speed", Mathf.Abs(move));
			
			rigidbody2D.velocity = new Vector2( move * maxSpeed, rigidbody2D.velocity.y );
			if( move > 0 && !facingRight ){
				Flip();
			}
			else if( move < 0 && facingRight ){
				Flip();
			}
		}
	}

	void MeleeAttack(){
		if( !stunned ){
			if (Input.GetButtonDown ("Fire3")) 
				isAttacking = true;
			if(Input.GetButtonUp("Fire3"))
				isAttacking = false;
			
			anim.SetBool ("isAttacking", isAttacking);
		}
	}

	//Flips player sprite accordingly to which side he is facing
	public void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void FixedUpdate(){
		if(!DEBUG && !networkController.isOwner)
			return;
		IsGrounded();
		MovePlayer();
	}

	// Update is called once per frame
	void Update () {
		if(!DEBUG && !networkController.isOwner)
			return;
		Jump();
		MeleeAttack();
	}
	
	private void Jump(){
		if( !stunned && grounded  && Input.GetButtonDown("Jump") && canJump){
			anim.SetBool( "Ground" , false );
			rigidbody2D.AddForce( new Vector2( 0, jumpForce ) );
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(dead)
			return;
		if (other.gameObject.tag == "GravityFieldTag")
		{
			Debug.Log(other.gameObject.tag);
			Debug.Log("Hit gravity field");
			rigidbody2D.gravityScale = -1;
		}

		if (other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();
			power.PowerActionEnter(gameObject, this);
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if (other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();
			power.PowerActionStay(gameObject, this);
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if(dead)
			return;
		if (other.gameObject.tag == "GravityFieldTag")
		{
			rigidbody2D.gravityScale = 1;
		}

		if (other.gameObject.tag == "Power")
		{
			Power power = other.gameObject.GetComponent<Power>();
			power.PowerActionExit(gameObject, this);
		}
	}


	void OnCollisionStay2D(Collision2D col ) {
		if(dead)
			return;
		if (col.gameObject.tag == "Power"){
			Power power = col.gameObject.GetComponent<Power>();
			power.PowerActionStay(gameObject, this);
		}
	}


	public void Die(){
		//IMPORTANT: This is here temporarily. We want this check in all collision functions.
		if(networkController.isOwner && dead == false){
			dead = true;
			Debug.Log ("I died again");

			//Here we call whatever events are subscribed to us.
			if(onDeath != null)
				onDeath(gameObject);
			//We don't need the next line any more
			//networkController.instanceManager.KillPlayer(gameObject);
		}
	}
}

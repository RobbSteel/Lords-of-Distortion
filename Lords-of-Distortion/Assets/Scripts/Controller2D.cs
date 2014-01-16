using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour {

	public float maxSpeed = 10f;
	public bool facingRight = false;
	public Animator anim;

	private bool grounded = false;
	public Transform groundCheck;
	private float groundRadius = 0.2f;
	public LayerMask groundLayer;
	public float jumpForce = 700f;

	NetworkController networkController;


	//Constantly checks if player is on the ground
	void IsGrounded(){
		grounded = Physics2D.OverlapCircle(groundCheck.position , groundRadius, groundLayer );
		anim.SetBool( "Ground", grounded );
	}

	//Needs to go in fixedUpdate since we use physics to move player.
	void MovePlayer(){
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

	//Flips player sprite accordingly to which side he is facing
	public void Flip(){
		LineRenderer renderer;
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		networkController = GetComponent<NetworkController>();
		isOwner = networkController.isOwner;
	}


	void FixedUpdate(){
		if(!networkController.isOwner)
			return;
		IsGrounded();
		MovePlayer();
	}

	// Update is called once per frame
	void Update () {
		if(!isOwner)
			return;
		Jump();
	}
	
	private void Jump(){
		if(grounded  && Input.GetButtonDown("Jump")){
			anim.SetBool( "Ground" , false );
			rigidbody2D.AddForce( new Vector2( 0, jumpForce ) );
		}
	}
}

using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour {

	public float maxSpeed = 10f;
	public bool facingRight = false;


	public Animator anim;

	bool grounded = false;
	public Transform groundCheck;
	float groundRadius = 0.2f;
	public LayerMask whatIsGround;
	public float jumpForce = 700f;

	private bool isOwner = false;


	//Constantly checks if player is on the ground
	void IsGrounded(){
		grounded = Physics2D.OverlapCircle(groundCheck.position , groundRadius, whatIsGround );
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
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		NetworkController remoteController = GetComponent<NetworkController>();
		isOwner = remoteController.isOwner;
	}


	void FixedUpdate(){
		if(!isOwner)
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

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){

		Vector3 syncPosition = Vector3.zero;
		bool syncFacing = false;

		if (stream.isWriting)
		{
			//if we have control over this entity, send out our positions to everyone else.
			syncPosition = transform.position;
			syncFacing = facingRight;
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncFacing);
		}
	}

}

using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour {
	[HideInInspector]
	public bool facingRight;
	[HideInInspector]
	public bool jump = false;

	public bool DEBUG;
	public float maxSpeed = 10f;
	public Animator anim;

	private bool dead = false;
	private bool grounded = false;
	public Transform groundCheck;
	private float groundRadius = 0.2f;
	public LayerMask groundLayer;
	public float jumpForce = 700f;
	public bool stunned;
	public bool snared = false;
	public bool canJump;

	public delegate void DieAction(GameObject gO);
	public static event DieAction onDeath; 

	public PSinfo infoscript;
	public GameObject DeathSpirit;

	NetworkController networkController;
	Hook myHook;

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
		myHook = GetComponent<Hook>();
		var psinfo = GameObject.Find("PSInfo");
		infoscript = psinfo.GetComponent<PSinfo>();
	}

	// Update is called once per frame
	void Update () {
		if(!DEBUG && !networkController.isOwner && !snared)
			return;
		Jump();
	}
	

	void FixedUpdate(){
		if(!DEBUG && !networkController.isOwner)
			return;
		IsGrounded();
		MovePlayer();

		// If player jumps
		if (jump) {
			// set the Jump animator trigger parameter
			anim.SetTrigger("Jump");

			//Add a vertical force to player
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));

			// player can't jump again until jump conditions from Update are satisfied
			jump = false;
		}
	}

	private void Jump(){
		if(!snared &&  !stunned && grounded  && Input.GetButtonDown("Jump") && canJump){
			jump = true;
		}
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
		if( !stunned && !snared ){
			//anim.SetFloat ( "vSpeed" , rigidbody2D.velocity.y );
			
			//to make jumping and changing direction is disabled
			//if(!grounded) return;

			
			float move = Input.GetAxis ( "Horizontal" );
			anim.SetFloat("Speed", Mathf.Abs(move));
			
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
		if(networkController.isOwner && dead)
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
		if(dead)
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
		//IMPORTANT: This is here temporarily. We want this check in all collision functions.
		if(dead == false){
			//Spawns the spirit upon player death
			Network.Instantiate(DeathSpirit, transform.position, transform.rotation, 0);
			dead = true;

			/*Upon Death, tell the DeadLord Script that the player is dead by setting
			the boolean to true*/
			var deadlord = GameObject.Find("DeadLordsScreen");
			var deadlordscript = deadlord.GetComponent<DeadLord>();
			deadlordscript.isDead = true;
			Debug.Log ("I died again");

			//Here we call whatever events are subscribed to us.
			if(onDeath != null)
				onDeath(gameObject);
			//We don't need the next line any more
			//networkController.instanceManager.KillPlayer(gameObject);
		}
	}

	void OnDisable(){
		myHook.DestroyHookPossible();
	}
}

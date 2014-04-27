using UnityEngine;
using System.Collections;

/*
 * The general outline for a melee attack (proposal 1):
 * 1. You press melee button, character begins playing animation and enables collider for hit detection.
 * 2. If you hit a player, that player is notified as fast as possible via RPC.
 * 3. All clients play the same feedback sound and animation for a melee hitting.
 * 4. That player's hit count goes up, something happens depending on the hit count:
 *    I. The player is marked once with something visual but not too obstrusive
 *    II. The player is knocked back. Upon landing, play a gloomy bell sound and mark him with something more noticeable. 
 *    III. The third hit kills the player. Leave room for double knockouts and stuff.
 */

public class Melee : MonoBehaviour {
	
	private Controller2D controller;// Reference to the PlayerControl script.
	private NetworkController networkController;
	private Animator anim;		// Reference to the Animator component.
	private Hook myhook;
	public GameObject meleeObject;
	public bool meleeDisable;

	[HideInInspector]


	public float meleeTimer = 0;

	public float coolDownTimer = 0.5f;
	public float damageDealt = 15f;

	public bool meleeing = false;

	void Awake()
	{
		// Setting up the references.
		anim = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();
		myhook = GetComponent<Hook>();
		networkController = GetComponent<NetworkController>();
		meleeObject.GetComponent<BoxCollider2D>().enabled = false;

	}
	
	void Update ()
	{
		//this.GetComponent<BoxCollider2D>().enabled = false;
		if ( !meleeDisable && meleeTimer <= 0 && !myhook.hookthrown && !controller.locked) {
			// If the fire button is pressed...
			//print("maggot");
			if (Input.GetButtonDown ("Melee") && !controller.stunned && networkController.isOwner) {
				startMelee ();
			}
		}
		else
			meleeTimer -= Time.deltaTime;
	}

	//Play animation on all clients.
	[RPC]
	void NotifyVisualMelee(){
		anim.SetTrigger ("Melee");
	}

	private void startMelee(){
		meleeing = true;

		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Melee Attack", transform.position);
		}

		if(!controller.OFFLINE)
		networkView.RPC ("NotifyVisualMelee", RPCMode.Others);

		// Enable Box Collider 2D
		meleeObject.GetComponent<BoxCollider2D>().enabled = true;
		controller.Snare();
		anim.SetTrigger ("Melee");
		AudioSource.PlayClipAtPoint( controller.meleeSfx , transform.position);
		meleeTimer =  coolDownTimer;


	}

	//Called by animator when the animation is done. Previously the collider was sticking around.
	public void StopMelee(){
		meleeObject.GetComponent<BoxCollider2D>().enabled = false;
		controller.FreeFromSnare();
		meleeing = false;
	}


	public void HandleCollsion(GameObject playerObject){
		// ... find the StunBar script and call the TakeDamage function.
		//playerObject.GetComponent<StunBar>().TakeDamage(damageDealt);
		playerObject.GetComponent<PlayerStatus>().AddHit(controller.facingRight);

	}
	
}

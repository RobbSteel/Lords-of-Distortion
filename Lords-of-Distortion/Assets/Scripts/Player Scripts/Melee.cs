using UnityEngine;
using System.Collections;
using InControl;

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
	
	public float damageDealt = 15f;

	public bool meleeing = false;

	const float MOVEMENT_WINDOW = .033f; //about 2 frames if running at 60fps
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
		if (!meleeDisable && !myhook.HitSomething && !controller.locked && !controller.crouching ) {
			// If the fire button is pressed...
			if (networkController.isOwner && !controller.stunned) {
				if(GameInput.instance.usingGamePad)
				{
					if(InputManager.ActiveDevice.Action3.WasPressed)
					{
						startMelee();
					}
				}
				else if(Input.GetKeyDown(KeyMapping.MeleeKey))
				{
					startMelee ();
				}
			}
		}

		if(meleeing){
			movementTime -= Time.deltaTime;
			if(movementTime <= 0f && !controller.snared){
				controller.Snare();
			}
		}
	}

	//Play animation on all clients.
	[RPC]
	void NotifyVisualMelee(){
		anim.SetTrigger ("Melee");
	}

	float movementTime = .033f;

	private void startMelee(){
		meleeing = true;

		if(Analytics.Enabled){
			GA.API.Design.NewEvent("Melee Attack", transform.position);
		}

		if(!controller.OFFLINE)
			networkView.RPC ("NotifyVisualMelee", RPCMode.Others);

		// Enable Box Collider 2D
		meleeObject.GetComponent<BoxCollider2D>().enabled = true;
		movementTime = MOVEMENT_WINDOW;
		anim.SetTrigger ("Melee");
		AudioSource.PlayClipAtPoint( controller.meleeSfx , transform.position);
	}

	//Called by animator when the animation is done. Previously the collider was sticking around.
	public void StopMelee(){
		meleeObject.GetComponent<BoxCollider2D>().enabled = false;
		controller.FreeFromSnare();
		meleeing = false;
	}
	
	public void HandleCollision(GameObject playerObject){
		// ... find the StunBar script and call the TakeDamage function.
		//playerObject.GetComponent<StunBar>().TakeDamage(damageDealt);
		playerObject.GetComponent<PlayerStatus>().AddHit(controller.facingRight);

	}
	
}

using UnityEngine;
using System.Collections;

public class Melee : MonoBehaviour {
	
	private Controller2D controller;// Reference to the PlayerControl script.
	private NetworkController networkController;
	private Animator anim;		// Reference to the Animator component.
	private Hook myhook;
	public GameObject meleeObject;
	[HideInInspector]


	public float meleeTimer = 0;

	public float coolDownTimer = 0.5f;
	public float damageDealt = 15f;

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
		if (meleeTimer <= 0 && !myhook.hookthrown) {
			// If the fire button is pressed...
			//print("maggot");
			if (Input.GetButtonDown ("Fire3") && !controller.stunned && networkController.isOwner) {
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
		audio.Play ();
	}

	private void startMelee(){

		networkView.RPC ("NotifyVisualMelee", RPCMode.Others);
		// Enable Box Collider 2D
		meleeObject.GetComponent<BoxCollider2D>().enabled = true;
		anim.SetTrigger ("Melee");
		audio.Play ();

		meleeTimer =  coolDownTimer;
	}

	//Called by animator when the animation is done. Previously the collider was sticking around.
	public void StopMelee(){
		meleeObject.GetComponent<BoxCollider2D>().enabled = false;
	}

	public void HandleCollsion(GameObject playerObject){
		// ... find the StunBar script and call the TakeDamage function.
		//playerObject.GetComponent<StunBar>().TakeDamage(damageDealt);
		playerObject.GetComponent<StunBar>().AddHit(controller.facingRight);
	}
	
}

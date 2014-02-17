using UnityEngine;
using System.Collections;

public class MeleewithoutNetwork : MonoBehaviour {
	
	private Controller2D controller;// Reference to the PlayerControl script.
	private NetworkController networkController;
	private Animator anim;					// Reference to the Animator component.
	private Hook myhook;
	[HideInInspector]
	public float meleeTimer = 0;

	public float coolDownTimer = 0.5f;
	public float damageDealt = 15f;

	void Awake()
	{
		// Setting up the references.
		anim = transform.root.gameObject.GetComponent<Animator>();
		controller = transform.root.GetComponent<Controller2D>();
		networkController = transform.root.GetComponent<NetworkController>();
		this.GetComponent<BoxCollider2D>().enabled = false;
		myhook = transform.root.GetComponent<Hook>();
	}
	
	void Update ()
	{
		if (meleeTimer <= 0 && !myhook.hookthrown) {
	
			// If the fire button is pressed...
			if (Input.GetButtonDown ("Fire3") && !controller.stunned) {
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
		this.GetComponent<BoxCollider2D>().enabled = true;
		anim.SetTrigger ("Melee");
		audio.Play ();

		meleeTimer =  coolDownTimer;
	}
	void OnTriggerEnter2D (Collider2D col) 
	{

		// If melee atack hits a player...
		 if(col.gameObject.tag == "Player"){
			
			// ... find the StunBar script and call the TakeDamage function.
			col.gameObject.GetComponent<PlayerStatus>().TakeDamage(damageDealt);
		
			Debug.Log ("Player hit");
		}
		// ...else melee hits nothing
		else
			Debug.Log ("Hit nothing");
		// Disable BoxCollider2D
		this.GetComponent<BoxCollider2D>().enabled = false;

	}

}

using UnityEngine;
using System.Collections;

public class Melee : MonoBehaviour {
	
	private Controller2D controller;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.

	public float meleeTimer = 0;
	public float coolDownTimer = 0.5f;

	void Awake()
	{
		// Setting up the references.
		anim = transform.root.gameObject.GetComponent<Animator>();
		controller = transform.root.GetComponent<Controller2D>();
	}

	void FixedUpdate ()
	{
		if (meleeTimer <= 0) {
			// If the fire button is pressed...
			if (Input.GetButtonDown ("Fire3")) {
				startMelee ();
			}
		}
		else
			meleeTimer -= Time.deltaTime;
	}
	private void startMelee(){
		anim.SetTrigger ("Melee");
		audio.Play ();
		meleeTimer =  coolDownTimer;
	}
}

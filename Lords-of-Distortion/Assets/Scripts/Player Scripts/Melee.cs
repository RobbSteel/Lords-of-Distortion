using UnityEngine;
using System.Collections;

public class Melee : MonoBehaviour {
	
	private Controller2D controller;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.

	public float meleeTimer = 0;
	public float coolDownTimer = 0.5f;
	public Rigidbody2D meleeHit;			// Prefab of the meleeHit

	void Awake()
	{
		// Setting up the references.
		anim = transform.root.gameObject.GetComponent<Animator>();
		controller = transform.root.GetComponent<Controller2D>();
	}

	void Update ()
	{
		if (meleeTimer <= 0) {
			// If the fire button is pressed...
			if (Input.GetButtonDown ("Fire3")) {
				//startMelee ();
				anim.SetTrigger ("Melee");
				audio.Play ();
				meleeTimer =  coolDownTimer;
				// If the player is facing right...
				if(controller.facingRight)
				{
					// ... instantiate melee hit facing right 
					Rigidbody2D meleeInstance = Instantiate(meleeHit, transform.position, Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody2D;
				}
				else
				{
					// Otherwise instantiate melee hit facing left 
					Rigidbody2D meleeInstance = Instantiate(meleeHit, transform.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
				}
			}
		}
		else
			meleeTimer -= Time.deltaTime;
	}
	/*
	private void startMelee(){
		anim.SetTrigger ("Melee");
		audio.Play ();
		meleeTimer =  coolDownTimer;
	}
*/
}

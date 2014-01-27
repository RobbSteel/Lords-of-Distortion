using UnityEngine;
using System.Collections;

public class Melee : MonoBehaviour {
	
	private Controller2D controller;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.
	
	void Awake()
	{
		// Setting up the references.
		anim = transform.root.gameObject.GetComponent<Animator>();
		controller = transform.root.GetComponent<Controller2D>();
	}
	
	
	void Update ()
	{
		// If the fire button is pressed...
		if(Input.GetButtonDown("Fire3"))
		{
			// ... set the animator Melee trigger parameter
			//animation["meleeAttack"].wrapMode = WrapMode.Once;
			anim.SetTrigger("Melee");
		}
	}
}

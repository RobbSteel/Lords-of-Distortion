using UnityEngine;
using System.Collections;

public class MeleeHit : MonoBehaviour {

	private Melee melee;
	private Controller2D onHitDeath;
	private Dummy onHit;
	void Start(){
		melee = transform.root.GetComponent<Melee>();
	}

	void OnTriggerEnter2D (Collider2D col) 
	{
		// If melee atack hits a player...
		if(col.gameObject.tag == "Player"){
			col.GetComponent<Controller2D>();
			//check to see if an effect is in place to be killed on melee hit
			onHitDeath = col.GetComponent<Controller2D>();
			melee.HandleCollision(col.gameObject);
			AudioSource.PlayClipAtPoint( onHitDeath.meleeHitSfx , transform.position);
			// Disable BoxCollider2D so that only one hit registers.
			GetComponent<BoxCollider2D>().enabled = false;
		
		}
	}
}

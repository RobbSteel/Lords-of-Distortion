using UnityEngine;
using System.Collections;

public class MeleeHit : MonoBehaviour {

	private Melee melee;

	void Start(){
		melee = transform.root.GetComponent<Melee>();
	}

	void OnTriggerEnter2D (Collider2D col) 
	{
		// If melee atack hits a player...
		if(col.gameObject.tag == "Player"){
			melee.HandleCollsion(col.gameObject);
			Debug.Log ("Player hit");
		}
		// ...else melee hits nothing
		else
			Debug.Log ("Hit something other than player");
		// Disable BoxCollider2D so that only one hit registers.
		GetComponent<BoxCollider2D>().enabled = false;
	}
}

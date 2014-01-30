using UnityEngine;
using System.Collections;

public class MeleeHit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	void OnTriggerEnter2D (Collider2D col) 
	{
		// If it hits an enemy...
		if(col.tag == "Enemy"){

			// ... find the Enemy script and call the Hurt function.
			col.gameObject.GetComponent<Enemy>().Hurt();

			Destroy(gameObject);
			Debug.Log ("Enemy hit, Melee hit destroyed");
		}
		// If it hits an enemy...
		else if(col.gameObject.tag == "Player"){
			
			// ... find the Enemy script and call the Hurt function.
			col.gameObject.GetComponent<StunBar>().TakeDamage(10f);
			
			Destroy(gameObject);
			Debug.Log ("Player hit, Melee hit destroyed");
		}
		// If melee hits nothing
		else if(col.gameObject.tag != "Player"){

			Destroy(gameObject);
			Debug.Log ("Hit nothing, Melee hit destroyed");
		}
	}
}

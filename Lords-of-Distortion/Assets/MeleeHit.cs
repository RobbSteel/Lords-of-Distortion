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
			Debug.Log ("Enemy hit, Melee hit destoryed");
		}
		else if(col.gameObject.tag != "Player"){
			Destroy(gameObject);
			Debug.Log ("Hit nothing, Melee hit destoryed");
		}
	}
}

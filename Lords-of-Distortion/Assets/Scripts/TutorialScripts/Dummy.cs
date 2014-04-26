using UnityEngine;
using System.Collections;

public class Dummy : MonoBehaviour {

	Vector2 knockBackLeft = new Vector2(200f, 200f);
	Vector2 knockBackRight = new Vector2(-200f, 200f);
	Controller2D playerHitDirection;
	void Awake(){

	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.CompareTag ("Melee")) {
			playerHitDirection = col.transform.root.GetComponent<Controller2D>();
			Debug.Log("Melee Dummy" );
			if(  !playerHitDirection.facingRight ){
				rigidbody2D.AddForce(knockBackRight);
			}else{
				rigidbody2D.AddForce(knockBackLeft);
			}
		}
	}
	
}

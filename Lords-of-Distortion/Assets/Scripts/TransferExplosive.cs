using UnityEngine;
using System.Collections;

public class TransferExplosive : MonoBehaviour {

	public float timer = 8;
	public GameObject playerstuck;
	private bool stickready = true;
	public float stickytimer = 2;
	private bool firststick = true;

	// Use this for initialization
	void Awake () {
	


	}
	
	// Update is called once per frame
	void Update () {
	
		//Blow up if the time is out
		if(timer <= 0){

			print("Boom");

		} else {

			//Do this only if the bomb can be transfered
			if(!stickready){
			
				stickytimer -= Time.deltaTime;

				if(stickytimer <= 0){

					stickytimer = 2;
					stickready = true;
				}
			}

			//If a player is stuck, follow them
			if(playerstuck != null){

			transform.position = Vector2.MoveTowards(transform.position, playerstuck.transform.position, 5);

			}

			timer -= Time.deltaTime;

		}
	}


	//Check for player collisions, until then roll around
	void OnTriggerEnter2D(Collider2D hit){

		
		if(hit.gameObject.tag == "Player"){


			//Check for the first player to be stuck so we don't get null references
			if(stickready && firststick){

				playerstuck = hit.gameObject;
				stickready = false;
				firststick = false;
				transform.rigidbody2D.velocity = Vector2.zero;
				collider2D.isTrigger = true;
			
			//Check for any future transitions of the bomb
			} else if(stickready && hit.gameObject != playerstuck) {
				print("getting to here");
				playerstuck = hit.gameObject;
				stickready = false;

			}
		}
	}


}
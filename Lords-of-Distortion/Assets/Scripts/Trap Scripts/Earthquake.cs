using UnityEngine;
using System.Collections;

public class Earthquake : Power {

	public float risetime;

	// Use this for initialization
	void Start () {
		risetime = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(risetime <= 0){

			transform.Translate(Vector3.down);

		} else {


			transform.Translate (Vector3.up);
			risetime -= Time.deltaTime;
		}

	}


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		if (GameObject.Find ("CollectData") != null) {
			GA.API.Design.NewEvent ("Earthquake Crushes", player.transform.position);
		}
		
		controller.Die ();
		
		//transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);
		//var player = GameObject.FindWithTag("Player");
		//GameObject user = GameObject.FindGameObjectWithTag("Player");
		// player.rigidbody.AddForce(Vector3.up * 10);
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		
	}
	public override void PowerActionExit(GameObject player, Controller2D controller)
	{
		
	}

	void OnTriggerEnter2D(Collider2D col)
	{

		risetime = 3;
		//if (col.gameObject.CompareTag ("killplatform")) {
		if(col.gameObject.tag =="killplatform"){

			var players = GameObject.FindGameObjectsWithTag("player");

			for(int i = 0; i < players.Length; i++){

				var currplayer = players[i];
				var networkscript = currplayer.GetComponent<NetworkController>();
				if(networkscript.isOwner){

					currplayer.rigidbody2D.AddForce(Vector3.up * 400);
				}
			}
		}
	}


}

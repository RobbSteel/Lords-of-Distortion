using UnityEngine;
using System.Collections;

public class Earthquake : Power {

	public float risetime;
	// Use this for initialization
	void Start () {
		risetime = 0;
		Destroy(gameObject, 10);
	}
	
	// Update is called once per frame
	void Update () {
	
		if(risetime <= 0){

			transform.Translate(Vector3.down * 8 * Time.deltaTime);

		} else {


			transform.Translate (Vector3.up * 1 * Time.deltaTime);
			risetime -= Time.deltaTime;
		}

	}


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		if (GameObject.Find ("CollectData") != null) {
			GA.API.Design.NewEvent ("Earthquake Crushes", player.transform.position);
		}
		
		controller.Die ();

	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		
	}
	public override void PowerActionExit(GameObject player, Controller2D controller)
	{
		
	}

	void OnTriggerEnter2D(Collider2D col)
	{

		//if (col.gameObject.CompareTag ("killplatform")) {
		if(col.transform.tag =="killplatform"){

			if(risetime <= 0){
				risetime = 2;
			}

			var players = GameObject.FindGameObjectsWithTag("Player");

			if(players.Length > 0){
			
				for(int i = 0; i < players.Length; i++){

					var currplayer = players[i];
					var networkscript = currplayer.GetComponent<NetworkController>();
					var controller = currplayer.GetComponent<Controller2D>();
							
					if(networkscript.isOwner){
							
						if(controller.grounded){
							if (GameObject.Find ("CollectData") != null) {
								GA.API.Design.NewEvent ("Earthquake Launches", currplayer.transform.position);
							}
						
							currplayer.rigidbody2D.AddForce(Vector3.up * 1500);
						}
					}
				}
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class Earthquake : Power {

	public float risetime;
	public GameObject groundshatter;
	public GameObject leafshatter;
	public int reps = 0;
	// Use this for initialization
	void Start () {
		risetime = 0;
		//Destroy(gameObject, 10);
	}
	
	// Update is called once per frame
	void Update () {
	
		if(risetime <= 0){

			transform.Translate(Vector3.down * 8 * Time.deltaTime);

		} else {


			transform.Translate (Vector3.up * 0.5f * Time.deltaTime);
			risetime -= Time.deltaTime;
		}

	}


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		if (Analytics.Enabled) {
			GA.API.Design.NewEvent ("Earthquake Crushes", player.transform.position);
		}

		if(risetime <= 0){
		controller.Die ();
		}
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
		
	}
	public override void PowerActionExit(GameObject player, Controller2D controller)
	{
		
	}

	[RPC]
	void QuakeParticle(Vector3 location, Quaternion rotate){

		Instantiate(groundshatter, location, rotate);

	}

	[RPC]
	void CrackPlatform(int index){

		var manager = GameObject.FindGameObjectWithTag("ArenaManager");
		var managerscript = manager.GetComponent<ArenaManager>();
		var platformlist = managerscript.platformlist;
		var platform = platformlist[index];
		Instantiate(leafshatter, platform.transform.position, platform.transform.rotation);
		Destroy(platform);
	}

	void DestroyPlatform(Collider2D col){

		if(Network.isServer){

		int index = 0;
		var manager = GameObject.FindGameObjectWithTag("ArenaManager");
		var managerscript = manager.GetComponent<ArenaManager>();
		var platformlist = managerscript.platformlist;

		for(int i = 0; i < platformlist.Count; i++){
			
			if(platformlist[i] == col.transform.gameObject){
				index = i;
			}
			
		 }
			Destroy(col.gameObject);
			Instantiate(leafshatter, col.transform.position, col.transform.rotation);
			networkView.RPC("CrackPlatform", RPCMode.Others, index);
		}

	}


	void OnTriggerEnter2D(Collider2D col)
	{


	


		//if (col.gameObject.CompareTag ("killplatform")) {
		if(col.transform.tag =="killplatform" || col.transform.tag == "Ground" || col.transform.tag == "SolidObject" || col.transform.tag == "movingPlatform"){

            if (col.transform.tag == "killplatform" || col.transform.tag == "movingPlatform")
            {

				DestroyPlatform(col);
			
			}

			if(risetime <= 0){
				Instantiate(groundshatter, transform.position, transform.rotation);
				reps += 1;
				risetime = 3;

			

			var players = GameObject.FindGameObjectsWithTag("Player");
			if(players.Length > 0){

				for(int i = 0; i < players.Length; i++){

					var currplayer = players[i];
					var networkscript = currplayer.GetComponent<NetworkController>();
					var playerstatus = currplayer.GetComponent<PlayerStatus>();
					var controller = currplayer.GetComponent<Controller2D>();
							
					if(networkscript.isOwner){
								
						if(controller.grounded){
							Instantiate(groundshatter, currplayer.transform.position, currplayer.transform.rotation);
							networkView.RPC("QuakeParticle", RPCMode.Others, currplayer.transform.position, currplayer.transform.rotation);
							if (Analytics.Enabled) {
								GA.API.Design.NewEvent ("Earthquake Launches", currplayer.transform.position);
							}

							playerstatus.TakeDamage(100.0f);
							playerstatus.GenerateEvent(this);
							currplayer.rigidbody2D.AddForce(Vector3.up * 1500);
						}
					}
				}
			}
				if(reps == 2){
					var renderer = gameObject.GetComponent<SpriteRenderer>();
					renderer.enabled = false;
					Destroy(gameObject, 2.0f);
				}
			
		  }


		}
	}


}

using UnityEngine;
using System.Collections;


public class FireBall : Power
{
	public float speed; 
	public AudioClip fireAwake;
	public AudioClip fireLoop;
	public bool redirect = false;

	public GameObject fireparticle;
	public GameObject firewall;
    public GameObject gField;

    void Start()
    {
		float angle = Mathf.Atan2(spawnInfo.direction.y, spawnInfo.direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		AudioSource.PlayClipAtPoint( fireAwake , transform.position);
		AudioSource.PlayClipAtPoint( fireLoop , transform.position);
        Destroy(gameObject, 10f);
    }
	// Update is called once per frame
    void Update ()
	{
		//Speed should not be dependant on framerate
	
		transform.Translate(Vector3.right * speed * Time.deltaTime);
		
    }

	[RPC]
	void FireExplosion(){
		print ("destroy fireball attempt");
		Destroy (gameObject);
		Instantiate(fireparticle, transform.position, transform.rotation);
	}

    public override void PowerActionEnter (GameObject player, Controller2D controller)
	{

		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Fireball Death", player.transform.position);
		}

		Instantiate(fireparticle, transform.position, transform.rotation);

		if(!OFFLINE)
			networkView.RPC("FireExplosion", RPCMode.Others);

		controller.Die(DeathType.FIRE);
		Destroy(gameObject);

	}

	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}

	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}

	[RPC]
	void BurnPlatform(int index){

		var manager = GameObject.FindGameObjectWithTag("ArenaManager");
		var managerscript = manager.GetComponent<ArenaManager>();
		var platformlist = managerscript.platformlist;
		var platform = platformlist[index];

		Instantiate(fireparticle, transform.position, transform.rotation);
		var newwall = (GameObject)Instantiate(firewall, platform.transform.position, Quaternion.AngleAxis(90,new Vector3(-1,0,0)));
		newwall.GetComponent<Firewall>().spawnInfo = new PowerSpawn(spawnInfo);
		Destroy (newwall, 3.0f);
		Destroy(gameObject);
		Destroy(platform);
	}

	void OnTriggerEnter2D(Collider2D col){
		print (col.transform);
        gField = GameObject.Find("gravityField(Clone)");
		if (gField != null) {
			if (col.transform.name == gField.name && gField.tag == "Power") {  
                Debug.Log("Col. Name" + col.transform.name);
                Debug.Log("GField. Name" + col.transform.name);
				transform.rotation = Quaternion.AngleAxis (90, new Vector3 (0, 0, 1));
			}
		}

		if(Network.isServer){
			if(col.transform.tag == "killplatform" || col.transform.tag == "movingPlatform"){
				int index = 0;
				Instantiate(fireparticle, transform.position, transform.rotation);
				var newwall = (GameObject)Instantiate(firewall, col.transform.position, Quaternion.AngleAxis(90,new Vector3(-1,0,0)));
				newwall.GetComponent<Firewall>().spawnInfo = new PowerSpawn(spawnInfo);
				var manager = GameObject.FindGameObjectWithTag("ArenaManager");

				var managerscript = manager.GetComponent<ArenaManager>();
				var platformlist = managerscript.platformlist;

					for(int i = 0; i < platformlist.Count; i++){

						if(platformlist[i] == col.transform.gameObject){
						    index = i;
						}
					
					}

				Destroy (newwall, 3.0f);
				Destroy(col.gameObject);
				Destroy(gameObject);
				networkView.RPC("BurnPlatform", RPCMode.Others, index);
			
			}
		}
	}

}


	

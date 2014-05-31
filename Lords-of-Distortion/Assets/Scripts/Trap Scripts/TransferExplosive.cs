using UnityEngine;
using System.Collections;

public class TransferExplosive : Power
{
	public float timer = 8;
	public GameObject playerstuck = null;
	public GameObject explosionPrefab;
	public bool stickready = true;
	const float TRANSFER_WAIT = 1f;
	public float stickytimer = TRANSFER_WAIT;
	public bool firststick = true;
	public bool playerdied = false;
	bool sentRPC = false;
	bool exploded = false;

	public GameObject CharMark;

	[RPC]
	void IThinkIGotStuck (NetworkMessageInfo info)
	{
		if (stickready)
        {
			networkView.RPC ("AttachToPlayer", RPCMode.Others, info.sender, firststick);
			AttachToPlayer (info.sender, firststick);
		}
	}

	[RPC] 
	void AttachToPlayer (NetworkPlayer player, bool firstTime)
	{
		if (firstTime) 
        {
			firststick = false;

			rigidbody2D.isKinematic = true;
			collider2D.isTrigger = true;
			transform.parent = null;
			//because we are now moving the rigidbody manually as opposed to letting physics do the work
			//make it kinematic.
		
		} else {
			//set hasbomb of previous player to false
			playerstuck.GetComponent<Controller2D> ().hasbomb = false;
		}
		GameObject playerObject = SessionManager.Instance.psInfo.GetPlayerGameObject (player);
		playerstuck = playerObject;
		playerObject.GetComponent<Controller2D> ().hasbomb = true;

		sentRPC = false;
		stickready = false;
	}

	
	//Note: Only the host can decide who got hit.
	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		//cant stick to yourself again. also wait for response rpc before sending another one.
		if (controller.hasbomb || sentRPC) {
				return;
		}
		if (stickready) {
			//No need to confirm getting stuck if you're the server.
			if (Network.isServer)
            {
				if (Analytics.Enabled) 
                {
						GA.API.Design.NewEvent ("Players Stuck", player.transform.position);
				}
			
				networkView.RPC ("AttachToPlayer", RPCMode.Others, Network.player, firststick);
				//do this second so that firstick isnt changed until the end.
				AttachToPlayer (Network.player, firststick);
			} else {
				//We call this incase two people get stuck at around the same time, so that bombs dont
				//stick to different characters on different clients. 
				//Those with lower ping at a disadvantage (since their message gets there first)
				// but its probably not important for now.
								
				if (Analytics.Enabled) {
						GA.API.Design.NewEvent ("Bomb Transfers", player.transform.position);
				}

				sentRPC = true;
				networkView.RPC ("IThinkIGotStuck", RPCMode.Server);
			}
		}
		
	}
	
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}
	
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{

	}

	public override void Initialize (PowerSpawn spawn)
	{
		this.spawnInfo = spawn;
		spawn.createEvents = false;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag != "Player" && col.gameObject.tag != "Power")
		{
			rigidbody2D.isKinematic = true;
			collider2D.isTrigger = true;
			transform.parent = col.transform;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.transform.name == "Boulder(Clone)" && col.transform.tag == "Power") 
		{  
			GameObject explosion = Instantiate (explosionPrefab, transform.position, Quaternion.identity) as GameObject;
			explosion.GetComponent<BlastRadius>().spawnInfo = new PowerSpawn(spawnInfo);
			Vector3 charMarkLoc = transform.position;
			charMarkLoc.z = 1.4f;
			var charmark = Instantiate (CharMark, charMarkLoc, Quaternion.identity) as GameObject;
			Destroy (gameObject);
		}
	}

	bool warned = false;
	// Update is called once per frame
	void  Update ()
	{
		if (timer <= 3 && !warned) 
        {
			Animator animator = GetComponent<Animator>();
			animator.SetBool("Warn", true);
			warned = true;
		}
		//Blow up if the time is out
		if (timer <= 0 && !exploded)
        {
			//var playercontroller = playerstuck.GetComponent<Controller2D>();
		    //playercontroller.hasbomb = false;
			GameObject explosion = Instantiate (explosionPrefab, transform.position, Quaternion.identity) as GameObject;
			exploded = true;
			if(playerstuck != null){
				var playercontroller = playerstuck.GetComponent<Controller2D>();
			    playercontroller.hasbomb = false;
			}
			explosion.GetComponent<BlastRadius>().spawnInfo = new PowerSpawn(spawnInfo);
			Vector3 charMarkLoc = transform.position;
            charMarkLoc.z = 1.4f;
			var charmark = Instantiate (CharMark, charMarkLoc, Quaternion.identity) as GameObject;
			Destroy (gameObject);
            
		} else {
			//Debug.Log ("Bomb ELSE~~~~~");
			//Do this only if the bomb cannot be transfered
			if (!stickready)
            {
				stickytimer -= Time.deltaTime;
				if (stickytimer <= 0) 
                {
					stickytimer = TRANSFER_WAIT;
					stickready = true;
				}
			}
		}

		//If a player is stuck, follow them
		if (playerstuck != null) 
        {
			var playercontroller = playerstuck.GetComponent<Controller2D>();

			if(playercontroller.dead)
            {
				playercontroller.hasbomb = false;
				GameObject explosion = Instantiate (explosionPrefab, transform.position, Quaternion.identity) as GameObject;
				explosion.GetComponent<BlastRadius>().spawnInfo = new PowerSpawn(spawnInfo);
				Vector3 charMarkLoc = transform.position;
				charMarkLoc.z = 1.4f;
				var charmark = Instantiate (CharMark, charMarkLoc, Quaternion.identity) as GameObject;
				Destroy (gameObject);
			}

			transform.position = playerstuck.transform.position;
				
		}  
		timer -= Time.deltaTime;
			
	}
}
	
	

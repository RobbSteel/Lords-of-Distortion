

using UnityEngine;
using System.Collections;

public class TransferExplosive : Power
{
	
		public float timer = 8;
		public GameObject playerstuck = null;
		public bool stickready = true;
		public float stickytimer = 2;
		public bool firststick = true;
		bool sentRPC = false;
		bool exploded = false;
		GameObject explode;

		[RPC]
		void IThinkIGotStuck (NetworkMessageInfo info)
		{
				if (stickready) {
						networkView.RPC ("AttachToPlayer", RPCMode.Others, info.sender, firststick);
						AttachToPlayer (info.sender, firststick);
				}
		}

		[RPC] 
		void AttachToPlayer (NetworkPlayer player, bool firstTime)
		{
				if (firstTime) {
						firststick = false;
						//because we are now moving the rigidbody manually as opposed to letting physics do the work
						//make it kinematic.
						rigidbody2D.isKinematic = true;
						collider2D.isTrigger = true;
				} else {
						//set hasbomb of previous player to false
						playerstuck.GetComponent<Controller2D> ().hasbomb = false;
				}
				GameObject playerObject = SessionManager.instance.gameInfo.GetPlayerGameObject (player);
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
						if (Network.isServer) {
								networkView.RPC ("AttachToPlayer", RPCMode.Others, Network.player, firststick);
								//do this second so that firstick isnt changed until the end.
								AttachToPlayer (Network.player, firststick);
						} else {
								//We call this incase two people get stuck at around the same time, so that bombs dont
								//stick to different characters on different clients. 
								//Those with lower ping at a disadvantage (since their message gets there first)
								// but its probably not important for now.
								print ("Entered bomb");
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
				//print("exited");
				//controller.hasbomb = false;
		}
	
		// Update is called once per frame
		void  Update ()
		{
				if (timer <= 3 && !exploded) {
						//anim.SetTrigger("BombExplo");
						var bombExpo = Instantiate (Resources.Load ("BombE"), transform.position, Quaternion.identity) as GameObject;
						Destroy (gameObject);
				
				}
				
				
				//Blow up if the time is out
				if (timer <= 0 && !exploded) {
	
						explode = Instantiate (Resources.Load ("BombExplosion"), transform.position, Quaternion.identity) as GameObject;
						var charmark = Instantiate (Resources.Load ("Charmark"), transform.position, Quaternion.identity) as GameObject;
						Destroy (explode, 1.0f);
						Destroy (gameObject);
            
				} else {
						Debug.Log ("Bomb ELSE~~~~~");
						//Do this only if the bomb cannot be transfered
						if (!stickready) {

								stickytimer -= Time.deltaTime;
				
								if (stickytimer <= 0) {
					
										stickytimer = 2;
										stickready = true;
								}
						}

				}

				//If a player is stuck, follow them
				if (playerstuck != null) {
				
						transform.position = playerstuck.transform.position;
				
				}
			
				timer -= Time.deltaTime;
			
		}
}
	
	

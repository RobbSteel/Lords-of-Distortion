using UnityEngine;
using System.Collections;

public class HookHit : MonoBehaviour {

	// Use this for initialization
	public bool hooked = false;
	public bool playerhooked = false;
	public GameObject shooter;
	public float timer = 1;
	public bool destroyed = false;
	public GameObject players;
	public LineRenderer lr;
	public Material rope;
	public NetworkController networkController;
	public Controller2D affectedPlayerC2D;
	public Vector3 targetPosition;
	void Awake(){

		destroyed = false;
		timer = timer / 5;
		lr = gameObject.AddComponent<LineRenderer>();
		lr.SetWidth(.1f, .1f);
		lr.material = rope;
		lr.sortingLayerName = "Player";
		lr.sortingOrder = 1;
	}


	void Update () {
	
		if(playerhooked == true){
		
			transform.position = Vector3.MoveTowards(transform.position, players.transform.position, 10);
		}

		if(timer > 0){
			timer -= Time.deltaTime;
		}else{
		
			destroyed = true;
		}

		lr.SetPosition(0, transform.position);
		if(shooter == null)
			Destroy (this.gameObject);
		else
			lr.SetPosition(1, shooter.transform.position);
	}

	//On collision stops the hook from moving and tells the player to come to the hook
	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag != "Player" && col.gameObject.tag != "Power" && col.gameObject.tag != "Melee"){

			print ("hello");
			rigidbody2D.velocity = Vector2.zero;
			hooked = true;

		}

		if(col.gameObject.tag == "Player" && col.gameObject != shooter && destroyed != true){
			NetworkController  affectedPlayerNC = col.gameObject.GetComponent<NetworkController>();
			affectedPlayerC2D = col.gameObject.GetComponent<Controller2D>();
			if(affectedPlayerNC.theOwner == Network.player){
				players = col.gameObject;
				affectedPlayerC2D.Snare();
				rigidbody2D.velocity = Vector2.zero;

				shooter.networkView.RPC ("HitPlayer", RPCMode.Others, transform.position);
				//TODO: call hitplayer locally instead of doing the following:
				//do what the remote rpc would do, but locally:
				playerhooked = true; 
				targetPosition = transform.position;
				print ("I'm hit!");
			}
		}
	}
}

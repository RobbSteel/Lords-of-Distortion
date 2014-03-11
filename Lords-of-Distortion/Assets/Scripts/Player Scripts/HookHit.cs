using UnityEngine;
using System.Collections;

public class HookHit : MonoBehaviour {

	// Use this for initialization
	public bool hooked = false;
	public bool playerhooked = false;
	public GameObject shooter;
	public float timer = 1;
	public bool destroyed = false;
	public GameObject hookedPlayer;
	public LineRenderer lr;
	public Material rope;
	public NetworkController networkController;
	public Controller2D affectedPlayerC2D;
	public Vector3 targetPosition;
	public Animator anim;

	public AudioClip hookHitSfx;
	public AudioClip hookWallHitSfx;

	void Awake(){

		destroyed = false;
		timer = timer / 5;
		lr = gameObject.AddComponent<LineRenderer>();
		lr.SetWidth(.1f, .1f);
		lr.material = rope;
		lr.sortingLayerName = "Player";
		lr.sortingOrder = 1;
		anim = GetComponent<Animator>();
	}


	void Update () {
	
		if(playerhooked == true){
			transform.position = Vector3.MoveTowards(transform.position, hookedPlayer.transform.position, 10);
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
		if(col.gameObject.tag != "Player" && col.gameObject.tag != "Power" && !col.isTrigger){

			if(GameObject.Find("CollectData") != null){
				GA.API.Design.NewEvent("Hooked Environment", col.transform.position);
			}
			//print ("hello");
			rigidbody2D.velocity = Vector2.zero;
			anim.SetTrigger("Hooked");
			hooked = true;
			//AudioSource.PlayClipAtPoint( hookWallHitSfx , transform.position );


		}

		if(col.gameObject.tag == "Player" && col.gameObject != shooter && destroyed != true){

			if(Network.isServer){

				if(GameObject.Find("CollectData") != null){
					GA.API.Design.NewEvent("Hook Hits", col.transform.position);
				}

				hookedPlayer = col.gameObject;
				NetworkController  affectedPlayerNC = hookedPlayer.GetComponent<NetworkController>();
			    
				affectedPlayerC2D = hookedPlayer.GetComponent<Controller2D>();
				affectedPlayerC2D.Hooked();
				rigidbody2D.velocity = Vector2.zero;
				targetPosition = transform.position;
				shooter.networkView.RPC ("HitPlayer", RPCMode.Others, transform.position, affectedPlayerNC.theOwner);
				playerhooked = true; 
				//AudioSource.PlayClipAtPoint( hookHitSfx , transform.position );


				shooter.GetComponent<Hook>().HitPlayerLocal(transform.position, affectedPlayerNC.theOwner);
				//TODO: make this animation appear on all players screens
				anim.SetTrigger("Hooked");

			}
		}
	}
}

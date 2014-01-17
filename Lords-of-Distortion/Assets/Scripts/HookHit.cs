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


	void Awake(){

		destroyed = false;
		timer = timer / 5;
		lr = gameObject.AddComponent<LineRenderer>();
		lr.SetWidth(.1f, .1f);
		lr.material = rope;
		lr.sortingLayerName = "player";
		lr.sortingOrder = 1;
	}


	void Update () {
	
		if(playerhooked == true){
		
			transform.position = Vector2.MoveTowards(transform.position, players.transform.position, 10);
		}

		if(timer > 0){
			timer -= Time.deltaTime;
		}else{
		
			destroyed = true;
		}
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, shooter.transform.position);
	}

	//On collision stops the hook from moving and tells the player to come to the hook
	void OnTriggerEnter2D(Collider2D col){

		if(col.gameObject.tag != "Player"){
			print (col.gameObject.tag);
			rigidbody2D.velocity = Vector2.zero;
			hooked = true;
			print ("hello");
		}

		if(col.gameObject.tag == "Player" && col.gameObject != shooter){
			print (col.gameObject.tag);
			players = col.gameObject;
			rigidbody2D.velocity = Vector2.zero;
			playerhooked = true;
			print ("hello2");
		}



	}




}

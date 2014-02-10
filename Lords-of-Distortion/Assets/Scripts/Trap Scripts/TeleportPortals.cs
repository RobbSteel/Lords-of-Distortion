using UnityEngine;
using System.Collections;

public class TeleportPortals : Power {


	public GameObject portalTo;
	private bool on;

	void Awake(){
		Debug.Log("Portal in level");
		on = true;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){
		if( on ){
			portalTo.GetComponent<TeleportPortals>().on = false;
			player.transform.position = portalTo.transform.position;
		}
		Debug.Log( "PORTAL" );
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){

	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){
		//portalTo.GetComponent<TeleportPortals>().entered = false;
		on = true;
	}
}

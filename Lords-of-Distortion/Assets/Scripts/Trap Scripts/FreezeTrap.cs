using UnityEngine;
using System.Collections;

public class FreezeTrap : Power {

	private PlayerStatus applyDmg;
	private float trapDuration;
	private GameObject frozenEffect;
	private Controller2D frozenplayer;
	private GameObject currentplayer;
	private bool used = false;

	public GameObject frozenEffectPrefab;

	void Awake(){

	}

	void Start(){
		trapDuration = 3f;
		Destroy(gameObject, trapDuration );
	}

	void Update(){

	}


	[RPC]
	void FreezeFollow(NetworkPlayer player){

		GameObject playerObject = SessionManager.instance.gameInfo.GetPlayerGameObject (player);
		var tempfreeze = (GameObject)Instantiate(frozenEffectPrefab ,playerObject.transform.position, Quaternion.identity);
		var playercontroller = playerObject.GetComponent<Controller2D>();
		applyDmg = playercontroller.GetComponent<PlayerStatus> ();
		applyDmg.Frozen();
		tempfreeze.GetComponent<FrozenEffect>().checkStun = playercontroller;
		print (tempfreeze.GetComponent<FrozenEffect>().checkStun);

	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){
        player.rigidbody2D.drag = 25;
		applyDmg = controller.GetComponent<PlayerStatus> ();
		applyDmg.Frozen();



		//to keep from duplicating the effect since we had 2 colliders 
		if ( !used ) {
			if(GameObject.Find("CollectData") != null){
				GA.API.Design.NewEvent("Times Frozen", player.transform.position);
			}
			used = true;
			frozenEffect = (GameObject)Instantiate(Resources.Load ("FrozenEffect"),player.transform.position, Quaternion.identity);
			frozenEffect.GetComponent<FrozenEffect> ().checkStun = controller;
			frozenplayer = controller;
			currentplayer = player;

			networkView.RPC("FreezeFollow", RPCMode.Others, Network.player);
		}

	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){
        player.rigidbody2D.drag = 25;
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){
        player.rigidbody2D.drag = 0;
	}
	
	void OnDestroy(){

	}
}

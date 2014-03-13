using UnityEngine;
using System.Collections;

public class FreezeTrap : Power {

	private PlayerStatus applyDmg;
	private float trapDuration;
	private GameObject frozenEffect;
	private Controller2D frozenplayer;
	private GameObject currentplayer;

	public GameObject frozenEffectPrefab;

	void Awake(){

	}

	void Start(){
		trapDuration = 1.5f;
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
		applyDmg = controller.GetComponent<PlayerStatus> ();
		applyDmg.Frozen();



		//to keep from duplicating the effect since we had 2 colliders 
		if (!controller.transform.FindChild ("FrozenEffect(Clone)")) {
			if(GameObject.Find("CollectData") != null){
				GA.API.Design.NewEvent("Times Frozen", player.transform.position);
			}
			frozenEffect = (GameObject)Instantiate(frozenEffectPrefab, player.transform.position, Quaternion.identity);
			frozenEffect.GetComponent<FrozenEffect> ().checkStun = controller;
			frozenplayer = controller;
			currentplayer = player;

			networkView.RPC("FreezeFollow", RPCMode.Others, Network.player);
		}

	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){

	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){

	}
	
	void OnDestroy(){

	}
}

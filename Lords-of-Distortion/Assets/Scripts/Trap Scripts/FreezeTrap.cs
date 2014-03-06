using UnityEngine;
using System.Collections;

public class FreezeTrap : Power {

	private PlayerStatus applyDmg;
	private float trapDuration;
	private GameObject frozenEffect;

	void Awake(){

	}

	void Start(){
		trapDuration = 1.5f;
	}

	void Update(){
		Destroy(gameObject, trapDuration );

	}
	
	public override void PowerActionEnter(GameObject player, Controller2D controller){
		applyDmg = controller.GetComponent<PlayerStatus> ();
		applyDmg.Frozen();

		//to keep from duplicating the effect since we had 2 colliders 
		if (!controller.transform.FindChild ("FrozenEffect(Clone)")) {
			frozenEffect = (GameObject)Network.Instantiate (Resources.Load ("FrozenEffect"),player.transform.position, Quaternion.identity, 1);
			frozenEffect.GetComponent<FrozenEffect> ().checkStun = controller;
		}

	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){

	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){

	}
	
	void OnDestroy(){

	}
}

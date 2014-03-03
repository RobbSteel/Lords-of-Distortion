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

		frozenEffect = (GameObject)Instantiate (Resources.Load ("FrozenEffect"));
		frozenEffect.GetComponent<FrozenEffect> ().checkStun = controller;


	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){

	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){

	}
	
	void OnDestroy(){

	}
}

using UnityEngine;
using System.Collections;

public class FreezeTrap : Power {

	private PlayerStatus applyDmg;
	public float trapDuration;

	void Awake(){

	}
	void Start(){
		trapDuration = 6f;
	}

	void Update(){
		Destroy(gameObject, trapDuration );

	}
	
	public override void PowerActionEnter(GameObject player, Controller2D controller){
		applyDmg = controller.GetComponent<PlayerStatus> ();
		applyDmg.Frozen();

	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){

	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){

	}
	
	void OnDestroy(){

	}
}

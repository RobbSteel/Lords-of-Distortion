using UnityEngine;
using System.Collections;

public class DefectionShield : Power {


	public bool insideExplosionRange;
	public float timer;
	public const float EXPLOSION_WAIT = 10f;
	public float defectionTimer = EXPLOSION_WAIT;
	public float defectionHitTimeReduction = 2f;
	public float defectionAnimationLength;
	private GameObject target;


	// Use this for initialization
	void Start () {
		insideExplosionRange = false;
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		DefectionShieldTimer();
	}

	private void DefectionShieldTimer(){
		timer += Time.deltaTime;
		if (timer >= defectionTimer) {
			Explode();
		}
	}

	//will take care of animation for bubble shield before it explodes
	//and destroying it
	public void Explode(){
		Destroy (this.gameObject , defectionAnimationLength );
	}

	public void DefectionSheildHit(){
		Debug.Log( "CurrentTime: " + timer + " TimeReduced: " + defectionHitTimeReduction );
		timer += defectionHitTimeReduction;
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){
		insideExplosionRange = true;
		target = player;
		controller.powerInvulnerable = true;
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){
		insideExplosionRange = true;
		controller.powerInvulnerable = true;
		target = player;
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){
		insideExplosionRange = false;
		target = null;
		controller.powerInvulnerable = false;

	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.transform.tag == "Power"){
			DefectionSheildHit();
			Destroy( col.gameObject );
		} 
	}

	void OnTriggerStay2D(Collider2D col){
		if(col.transform.tag == "Power"){
			//col.enabled = false;
			Destroy( col.gameObject );
			DefectionSheildHit();

		} 
	}

	void OnDestroy(){
		if (insideExplosionRange) {
			Destroy (target);
		} 
	}
}

﻿using UnityEngine;
using System.Collections;

public class DefectionShield : Power {


	public bool insideExplosionRange;
	public float timer;
	public const float EXPLOSION_MAX_WAIT = 10f;
	private float defectionTimer = EXPLOSION_MAX_WAIT;
	public float defectionHitTimeReduction = 2f;
	public GameObject explosionPrefab;
	public GameObject absorbPowerEffect;
	public Color shieldshaderColor;
	private GameObject target;
	private Color currentColor;
	//needed for collision checking with trigger system to detect 2 objects that are static one needs to be moving
	//in order to detect each other.
    private Controller2D playerController;
	private bool switchMove;
	private Vector3 original;
	private Vector3 end;

	public GameObject CharMark;
	public AudioClip wubwub;

	// Use this for initialization
	void Start () {

		this.renderer.material.SetColor ("_TintColor", shieldshaderColor);
        particleSystem.renderer.sortingLayerName = "Foreground";
		currentColor = shieldshaderColor;
		insideExplosionRange = false;
		timer = 0;
		original = this.transform.position;
		end = this.transform.position + Vector3.left* 0.1f;
		audio.loop = true;

	}
	
	// Update is called once per frame
	void Update () {
		DefectionShieldTimer();


	}

	//might not need
	void Move(){
		if (switchMove) {
			this.transform.position += Vector3.up;
			switchMove = false;
		} else {
			this.transform.position -= Vector3.up;
			switchMove = true;
		}
	}
	
	private void DefectionShieldTimer(){
		timer += Time.deltaTime;
		//sets correct color over time for shader
		currentColor = Color.Lerp (shieldshaderColor, Color.red, timer/defectionTimer);
		this.renderer.material.SetColor ("_TintColor", currentColor);
		if (timer >= defectionTimer) {
			Explode();
		}
	}

	//will take care of animation for bubble shield before it explodes
	//and destroying it
	public void Explode(){
		GameObject explosion = Instantiate (explosionPrefab, transform.position, Quaternion.identity) as GameObject;
        explosion.GetComponent<BlastRadius>().spawnInfo = new PowerSpawn(spawnInfo);
        Vector3 charMarkLoc = transform.position;
        charMarkLoc.z = 1.4f;
		var charmark = Instantiate(CharMark, charMarkLoc, Quaternion.identity) as GameObject;
        Destroy(gameObject);
		this.collider2D.enabled = false;
	}

	public void DefectionSheildHit(){
		defectionTimer -= defectionHitTimeReduction;
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){
		insideExplosionRange = true;
		target = player;
		controller.powerInvulnerable = true;
        if (playerController == null)
            playerController = controller;
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){
		insideExplosionRange = true;
		controller.powerInvulnerable = true;
		target = player;
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){
		target = null;
        insideExplosionRange = false;
		controller.powerInvulnerable = false;
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.transform.tag == "Power"){
			Destroy( col.gameObject );
			absorbPowerEffect.GetComponent<ParticleSystem>().startColor = renderer.material.GetColor ("_TintColor");
			GameObject absorbEffect = Instantiate (absorbPowerEffect, col.transform.position, Quaternion.identity) as GameObject;
			DefectionSheildHit();
		} 
	}

	void OnTriggerStay2D(Collider2D col){
		if(col.transform.tag == "Power"){
			Destroy( col.gameObject );
			absorbPowerEffect.GetComponent<ParticleSystem>().startColor = renderer.material.GetColor ("_TintColor");
			GameObject absorbEffect = Instantiate (absorbPowerEffect, col.transform.position, Quaternion.identity) as GameObject;
			DefectionSheildHit();
		} 
	}

	void OnDestroy(){
		if( playerController != null )
			playerController.powerInvulnerable = false;

		if (insideExplosionRange) {
            playerController.Die(DeathType.EXPLOSION);
		} 
	}
}

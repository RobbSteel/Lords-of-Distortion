using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmokeBomb : Power {

	public float duration;
	private Camera levelCamera;
	private Camera newRenderCam;
	private string reducedVisionLayer;
	private string playerLayer;
	private string powerLayer;
	private string otherPlayerLayer;
	private string powerTag;
	private string playerTag;
	private string powerUpTag;
	private string cameraFadeInFunctionName;
	private string cameraFadeOutFunctionName;
	private List<GameObject> targets;
	private ParticleSystem smoke;



	void Awake(){
		targets = new List<GameObject> ();
		cameraFadeInFunctionName = "fadeIn";
		cameraFadeOutFunctionName = "fadeOut";
		playerTag = "Player";
		powerTag = "Power";
		powerUpTag = "PowerUp";
		playerLayer = "Player";
		powerLayer = "PowersLayer";
		otherPlayerLayer = "OtherPlayer";
		reducedVisionLayer = "ReducedVision";
		levelCamera = GameObject.Find("Main Camera").camera;
		//adds component if it isnt there
		if (levelCamera.GetComponent<FadeInOut> () == null)
						levelCamera.gameObject.AddComponent ("FadeInOut");

		smoke = GetComponent<ParticleSystem>();
		smoke.enableEmission = true;
	}
	
	// Use this for initialization
	void Start () {
		Destroy(gameObject, duration);
	}
	
	// Update is called once per frame
	void Update () {
		//Floating ();
	}

	void Floating(){
		if( this.rigidbody2D.velocity.y >= 0 )
		rigidbody2D.AddForce (Vector2.up);
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){
		if (!targets.Contains (player)) {
			targets.Add (player);
		}
		player.layer = LayerMask.NameToLayer(reducedVisionLayer);
		this.gameObject.layer = LayerMask.NameToLayer (reducedVisionLayer);
		visionReduced ();
	}

	public override void PowerActionStay(GameObject player, Controller2D controller){

	}

	public override void PowerActionExit(GameObject player, Controller2D controller){
		targets.Remove(player);
		this.gameObject.layer = LayerMask.NameToLayer (reducedVisionLayer);
		player.layer = LayerMask.NameToLayer ( playerLayer );
		visionReEnabled ();
	}

	private void visionReduced(){
		levelCamera.SendMessage (cameraFadeOutFunctionName);
		levelCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(powerLayer));
		levelCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(playerLayer));
		levelCamera.cullingMask &= ~(1 << LayerMask.NameToLayer (otherPlayerLayer));
	}

	private void visionReEnabled(){
		levelCamera.SendMessage (cameraFadeInFunctionName);
		levelCamera.cullingMask |= 1 << LayerMask.NameToLayer(powerLayer);
		levelCamera.cullingMask |= 1 << LayerMask.NameToLayer(playerLayer);
		levelCamera.cullingMask |= 1 << LayerMask.NameToLayer(otherPlayerLayer);
	}

	void OnTriggerEnter2D( Collider2D col ){
		if (col.CompareTag (powerTag) || col.CompareTag(powerUpTag)) {
			if( !targets.Contains( col.gameObject )){
				targets.Add(col.gameObject);
			}
			col.gameObject.layer = LayerMask.NameToLayer(reducedVisionLayer);
		}
	}

	void OnTriggerStay2D( Collider2D col ){
		Debug.Log ("POWER DETECTED WITHINSMOKE: " + col.name);
	}

	void OnCollisionStay2D( Collision2D col ){
		Debug.Log ("Collision POWER DETECTED WITHINSMOKE: " + col.gameObject.name);
	}

	void OnTriggerExit2D( Collider2D col ){
		if (col.CompareTag (powerTag) || col.CompareTag(powerUpTag)) {
			targets.Remove (col.gameObject);
			col.gameObject.layer = LayerMask.NameToLayer(powerLayer);
		}
	}

	void OnDestroy(){
		levelCamera.SendMessage (cameraFadeInFunctionName);
		visionReEnabled ();
		resetTargetLayers ();
	}

	void resetTargetLayers(){
		foreach (GameObject collided in targets) {
			if( collided != null ){
				if( collided.CompareTag( powerTag )){
					collided.layer = LayerMask.NameToLayer(powerLayer);
				}
				if( collided.CompareTag( playerTag )){
					collided.layer = LayerMask.NameToLayer(playerLayer);
				}
			}
		}
	}

}

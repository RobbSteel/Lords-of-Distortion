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

	//needed for collision checking with trigger system to detect 2 objects that are static one needs to be moving
	//in order to detect each other.
	private bool switchMove;
	private Vector3 original;
	private Vector3 end;
	private bool playerEnteredSmoke;
	private GameObject trackPlayerDeath;
	public AudioClip smokebomb;
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
		playerEnteredSmoke = true;
	}
	
	// Use this for initialization
	void Start () {
        particleSystem.renderer.sortingLayerName = "Foreground";
		Destroy(gameObject, duration);
		original = this.transform.position;
		end = this.transform.position + Vector3.left* 0.1f;
		audio.loop = true;
	}
	
	// Update is called once per frame
	void Update () {
		//needs to move a a very minium speed otherwise it wonts colide with traps
		//that are static sense both are static one needs to be moving in order to cause a collision
		this.transform.position = Vector3.Lerp( original , end ,Mathf.PingPong(Time.time , 1f));
		ResetOnPlayerDeath ();
	}

	//to reset if player dies within the smoke
	void ResetOnPlayerDeath(){
		if( !playerEnteredSmoke  ){
				playerEnteredSmoke = true;
				levelCamera.SendMessage (cameraFadeInFunctionName);
				visionReEnabled ();
				resetTargetLayers ();
		}
	}

	void Move(){
		if (switchMove) {
			this.transform.position += Vector3.up;
			switchMove = false;
		} else {
			this.transform.position -= Vector3.up;
			switchMove = true;
		}
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){
		if (!targets.Contains (player)) {
			targets.Add (player);
		}
		player.layer = LayerMask.NameToLayer(reducedVisionLayer);
		this.gameObject.layer = LayerMask.NameToLayer (reducedVisionLayer);
		visionReduced ();
		if(Analytics.Enabled){
			GA.API.Design.NewEvent("Smoke Blinds", player.transform.position);
		}
		controller.move = -controller.move;
	}

	public override void PowerActionStay(GameObject player, Controller2D controller){
	}

	public override void PowerActionExit(GameObject player, Controller2D controller){
		targets.Remove(player);
		this.gameObject.layer = LayerMask.NameToLayer (reducedVisionLayer);
		player.layer = LayerMask.NameToLayer ( playerLayer );
		visionReEnabled ();
	
		controller.move = -controller.move;

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
				col.gameObject.layer = LayerMask.NameToLayer(reducedVisionLayer);
				SetChildrensLayer( col.gameObject , reducedVisionLayer );
			}
		}

	}

	void OnTriggerStay2D( Collider2D col ){
		if( col.CompareTag(playerTag) ){
			playerEnteredSmoke = true;
		}

		if (col.CompareTag (powerTag) || col.CompareTag(powerUpTag)) {
			//Debug.Log ("POWER DETECTED WITHINSMOKE: " + col.name);

			if( !targets.Contains( col.gameObject )){
				targets.Add(col.gameObject);
				col.gameObject.layer = LayerMask.NameToLayer(reducedVisionLayer);
				SetChildrensLayer( col.gameObject , reducedVisionLayer );
			}
		}
	}

	void OnTriggerExit2D( Collider2D col ){
		if (col.CompareTag (powerTag) || col.CompareTag(powerUpTag)) {
			targets.Remove (col.gameObject);
			col.gameObject.layer = LayerMask.NameToLayer(powerLayer);
			SetChildrensLayer( col.gameObject , powerLayer );
		}
		if(col.CompareTag(playerTag)){
			playerEnteredSmoke = false;
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
					SetChildrensLayer( collided , powerLayer );
				}
				if( collided.CompareTag( playerTag )){
					collided.layer = LayerMask.NameToLayer(playerLayer);
				}
			}
		}
	}

	public static void SetChildrensLayer(GameObject go, string layer) {
		if (go == null) return;
		foreach (Transform trans in go.GetComponentsInChildren<Transform>(true)) {
			trans.gameObject.layer = LayerMask.NameToLayer(layer);
		}
		
	}

}

using UnityEngine;
using System.Collections;

public class StunBar : MonoBehaviour {


	public float maxStun = 100f;
	public float currentStunMeter = 50f;
	public float regenAmount = 10f;
	public float regenCooldown = 5f;
	public float regenTimer = 0f;
	public float stunTimer;
	public float stunWait;
	public float UIoffsetX;
	public float UIoffsetY;

	private Controller2D playerControl;		// Reference to the PlayerControl script.
	private GameObject UI;					// Reference UI GUI
	private UISlider stunBarUI;				// Reference UI slider values

	//setup references and create UI stunbar
	void Awake(){
		playerControl = GetComponent<Controller2D>();
		UI = (GameObject)Instantiate( Resources.Load( "StunBar" ) );
		stunBarUI = UI.GetComponent<UISlider>();
	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		RegenBar();
		UpdateHealthBar();
		CheckIfStunned();
		UpdateStunBarPosition();
	}

	//sets position of stunbar correctly on player
	void UpdateStunBarPosition(){
		Vector3 playersPos = transform.position;
		Vector3 screenPos = GameObject.Find("Main Camera").camera.WorldToScreenPoint( playersPos );
		float screenHeight = Screen.height;
		float screenWidth = Screen.width;
		screenPos.x -= (screenWidth / 2.0f);
		screenPos.y -= (screenHeight / 2.0f);

		screenPos.x += UIoffsetX;
		screenPos.y += UIoffsetY;

		stunBarUI.transform.localPosition = screenPos;

	}

	[RPC]
	void NotifyDamageTaken(float dmgTaken){
		ApplyDamage(dmgTaken);
	}

	private void ApplyDamage(float dmgTaken){
		if (!playerControl.stunned) {
			currentStunMeter += dmgTaken;
			if (currentStunMeter >= maxStun)
				Stun (); //this will only matter if called on the client which controls the player to be stunned
		}
	}


	//allows other objects to appliy a specific amount of damage
	public void TakeDamage( float dmgTaken ){
		//Don't worry about lag compensation for now and just notify all other of the stun bar being raised.
		//TODO: Have stun bar values more synchronized, because bar may decay differently on other clients.
		networkView.RPC("NotifyDamageTaken", RPCMode.Others, dmgTaken);
		ApplyDamage(dmgTaken);
	}

	//checks if player is stun then applies timer and grants player actions once time has past
	private void CheckIfStunned(){
		if( playerControl.stunned == true ){
			stunTimer += Time.deltaTime;

			if( stunTimer >= stunWait ){
				playerControl.stunned = false;
				stunTimer = 0;
				currentStunMeter = 0;
			}

		}
	}

	public void Stun(){
		playerControl.stunned = true;
		audio.Play ();
		stunTimer = 0;
	}
	
	public void Stun( float stunTime ){
		playerControl.stunned = true;
		stunTimer = 0;
		stunWait = stunTime;
	}

	//Only runs if your stun bar is injured
	//slowly regenerates your stun bar
	void RegenBar(){
		if( !playerControl.stunned ){
			if( currentStunMeter > 0 ){
				if( regenTimer > regenCooldown ){
					regenTimer = 0;

					if( currentStunMeter - regenAmount <= 0 ){
						currentStunMeter = 0;
					}else
						currentStunMeter -= regenAmount;
					}
				else{
					regenTimer += Time.deltaTime;
				}
			}
		}
	}



	//Updates StunBar UI and tints color relative to danger
	void UpdateHealthBar(){
		stunBarUI.value = currentStunMeter/maxStun;
		stunBarUI.foregroundWidget.color = Color.Lerp( Color.green , Color.red, currentStunMeter  );
	}

	//Ondestroy delete Stunbar Ui
	void OnDestroy(){
		Destroy( UI );
	}

	void OnCollisionEnter2D( Collision2D col ){
		
	}

} 

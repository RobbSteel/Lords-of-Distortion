﻿using UnityEngine;
using System.Collections;

public class StunBar : MonoBehaviour {


	public float maxStun = 100f;
	public float currentStunMeter = 50f;
	public float regenAmount = 15f;
	public float regenCooldown = 5f;
	public float regenTimer = 0f;
	//public float stunTimer;
	public float stunWait;
	public float UIoffsetX;
	public float UIoffsetY;
	public float recoverRate;

	private Controller2D playerControl;		// Reference to the PlayerControl script.
	private GameObject UI;					// Reference UI GUI
	private UISlider stunBarUI;				// Reference UI slider values
	private Camera levelCamera;
	private bool horizontalPressedUp;
	private bool horizontalPressedDown;
	private int horizontalMoveCheck;		//tracks horizontal current key
	//setup references and create UI stunbar

	int hitCount = 0;
	public GameObject hitMarks;
	SpriteRenderer hitMarkSprites;
	public Sprite firstMark, secondMark, thirdMark;

	void Awake(){
		recoverRate = 10f;
		horizontalPressedUp = false;
		horizontalPressedDown = false;
		playerControl = GetComponent<Controller2D>();
		/*
		UI = (GameObject)Instantiate( Resources.Load( "StunBar" ) );
		stunBarUI = UI.GetComponent<UISlider>();
	*/
		hitMarkSprites = hitMarks.GetComponent<SpriteRenderer>();

		levelCamera = GameObject.Find("Main Camera").camera;
	}

	void Start(){
		hitMarkSprites.enabled = false;
	}

	//this function acts as unitys input keydown and up for "Horizontal" input
	//**unity does not have this functionality yet needed to do this way 
	//**because if we change movement for keys it wont apply correctly 
	void StunRecover(){
		horizontalMoveCheck = (int)Input.GetAxisRaw("Horizontal");
		if( horizontalMoveCheck < 0 ){
			if(!horizontalPressedDown){
				currentStunMeter -= recoverRate;
				horizontalPressedDown = true;
			}
			if( horizontalPressedUp ){
				horizontalPressedUp = false;
			}
		}
		else if( horizontalMoveCheck > 0 ){
			if( !horizontalPressedDown){
				currentStunMeter -= recoverRate;
				horizontalPressedDown = true;
			}
			if( horizontalPressedUp ){
				horizontalPressedUp = false;
			}
		}
		else if( horizontalMoveCheck == 0 ){
			if( horizontalPressedUp ){
				horizontalPressedUp = false;
			}
			if( horizontalPressedDown ){
				horizontalPressedDown = false;
			}
		}

	}

	//Updates StunBar UI and tints color relative to danger
	void UpdateHealthBar(){
		stunBarUI.value = currentStunMeter/maxStun;
		stunBarUI.foregroundWidget.color = Color.Lerp( Color.yellow , Color.red, stunBarUI.value  );
	}

	//sets position of stunbar correctly on player
	void UpdateStunBarPosition(){
		Vector3 playersPos = transform.position;
		Vector3 screenPos = levelCamera.WorldToScreenPoint( playersPos );
		float screenHeight = Screen.height;
		float screenWidth = Screen.width;

		screenPos.x -= (screenWidth / 2.0f);
		screenPos.y -= (screenHeight / 2.0f);

		screenPos.x += UIoffsetX;
		screenPos.y += UIoffsetY;

		stunBarUI.transform.localPosition = screenPos;

	}

	
	// Update is called once per frame
	void Update () {
		RegenBar();
		//UpdateHealthBar();
		CheckIfStunned();
		//UpdateStunBarPosition();
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

	bool knockBackPending = false;

	Vector2 sideForce;

	void FixedUpdate(){
		//push the character sideways
		if(knockBackPending){
			//player should be in air by now, so disable movement
			playerControl.KnockBack();
			knockBackPending = false;
			hitMarkSprites.enabled = false;
			networkView.RPC ("VisualHitIndicator", RPCMode.Others, 0);
			rigidbody2D.AddForce(sideForce);
		}
	}

	Vector2 upForce = new Vector2(0f, 300f);
	//Push the character up on this step
	void BeginKnockBack(float flip){
		rigidbody2D.AddForce(upForce);

		sideForce = new Vector2(3000f * flip, 100f);
		knockBackPending = true;
	}


	[RPC]
	void NotifyHit(bool fromLeftSide){
		//ignore hits if the player is already knocked back
		if(playerControl.knockedBack){
			return;
		}
		hitCount++;
		networkView.RPC ("VisualHitIndicator", RPCMode.Others, hitCount);
		VisualHitIndicator(hitCount);
		if(hitCount >= 3){
			hitCount= 0;
			float flip = 1f;
			if(!fromLeftSide)
				flip = -1f;

			BeginKnockBack(flip);
		}

	}

	[RPC]
	void VisualHitIndicator(int hits){
		hitMarkSprites.enabled = true;

		switch(hits){
		case 0:
			hitMarkSprites.enabled = false;
			break;
		case 1:
			hitMarkSprites.sprite = firstMark;
			break;
		case 2:
			hitMarkSprites.sprite = secondMark;
			break;
		case 3:
			hitMarkSprites.sprite = thirdMark;
			break;
		}

	}

	//allows other objects to appliy a specific amount of damage
	public void TakeDamage( float dmgTaken ){
		//Don't worry about lag compensation for now and just notify all other of the stun bar being raised.
		//TODO: Have stun bar values more synchronized, because bar may decay differently on other clients.
		networkView.RPC("NotifyDamageTaken", RPCMode.Others, dmgTaken);
		ApplyDamage(dmgTaken);
	}

	public void AddHit( bool fromLeftSide){
		networkView.RPC ("NotifyHit", GetComponent<NetworkController>().theOwner, fromLeftSide);
	}

	//checks if player is stun and then applies stunRecover
	private void CheckIfStunned(){
		if( playerControl.stunned == true ){
			StunRecover();
			if( currentStunMeter <= 0 ){
				playerControl.stunned = false;
				playerControl.anim.SetBool("stunned", false);
				currentStunMeter = 0;
			}
		}
	}

	public void Stun(){
		playerControl.stunned = true;
		playerControl.anim.SetBool("stunned", true);
		audio.Play ();
	}
	
	public void Stun( float newRecoverRate ){
		playerControl.stunned = true;
		currentStunMeter = maxStun;
		recoverRate = newRecoverRate;
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





	//Ondestroy delete Stunbar Ui
	void OnDestroy(){
		Destroy( UI );
	}

} 

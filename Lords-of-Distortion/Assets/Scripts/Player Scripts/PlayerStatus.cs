﻿using UnityEngine;
using InControl;
using System.Collections;

public class PlayerStatus : MonoBehaviour {
	

	//varible to adjust stunbar & stun
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
	public float frozenrecover;

    //variables for Plague
    public bool insidePlague = false;
    private float plagueAmount = 0f;
    private float plagueTimer = 0f;

	//Metrics for death
	public float timegrav;
	public float timesmoke;
	public float timehook;
	public float timemelee;
	
	int hitCount = 0;
	//public GameObject hitMarks;
	//SpriteRenderer hitMarkSprites;
	//public Sprite firstMark, secondMark, thirdMark;
	public GameObject punchParticles;
	public GameObject shieldParticles;
	bool knockBackPending = false;
	Vector2 sideForce;
    Vector2 velocityChange;
	public float MashIconYOffset;
	private GameObject MashIcon;
	private ParticleSystem punchEffect;
	private ParticleSystem shield;

	private Controller2D playerControl;		// Reference to the PlayerControl script.
	private GameObject UI;					// Reference UI GUI
	private UISlider stunBarUI;				// Reference UI slider values
	private Camera levelCamera;

	private NetworkController networkController;
    //public UISprite shieldIcon;

	public delegate void PlayerAffected(NetworkPlayer player, PlayerEvent playerEvent);
	public static event PlayerAffected eventAction;


	void Awake(){
        //shieldIcon = GameObject.Find("UI-Passive").GetComponent<UISprite>();
		recoverRate = 10f;
		playerControl = GetComponent<Controller2D>();

		/*
		UI = (GameObject)Instantiate( Resources.Load( "StunBar" ) );
		stunBarUI = UI.GetComponent<UISlider>();
		*/

		MashIcon = (GameObject)Instantiate (Resources.Load ("MashAlertIcon"));
		MashIcon.SetActive (false);

		//hitMarkSprites = hitMarks.GetComponent<SpriteRenderer>();
		punchEffect = punchParticles.GetComponent<ParticleSystem>();
		shield = shieldParticles.GetComponent<ParticleSystem>();
		shield.enableEmission = false;
		levelCamera = GameObject.Find("Main Camera").camera;
	}

	void Start(){
		networkController = GetComponent<NetworkController>();
		//hitMarkSprites.enabled = false;
	}

	//used to turn on MashAlertIcon
	void TurnOnMashAlert(){
		MashIcon.SetActive(true);
		Vector3 playersPos = transform.position;
		playersPos.y += MashIconYOffset;
		MashIcon.transform.localPosition = playersPos;
	}

	//used to turn off MashAlertIcon
	void TurnOffMashAlert(){

		MashIcon.SetActive (false);
	}

	public void DestroyMashIcon(){
		playerControl.stunned = false;
		Destroy( MashIcon );
	}

	void StunRecover()
	{
		if(GameInput.instance.usingGamePad)
		{
			InputDevice device = InputManager.ActiveDevice;
			if(device.LeftStick.HasChanged || device.DPad.HasChanged)
			{
				currentStunMeter -= recoverRate * 1.1f; //it's harder to do this so increase recover rate
			}
			else if(device.AnyButton.WasPressed){
				currentStunMeter -= recoverRate/2f; //incredibily easy
			}
		}
		else
		{
			if(Input.GetKeyDown(KeyMapping.LeftKey) || Input.GetKeyDown(KeyMapping.RightKey))
			{
				currentStunMeter -= recoverRate;
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
	
    public void AddPlague(float amount)
    {
        plagueAmount += amount;
        PlagueUpdate();
    }

    public void RemovePlague()
    {
        plagueAmount = 0;
        PlagueUpdate();
    }

    public float PlagueAmount()
    {
        return plagueAmount;
    }

    private void PlagueUpdate()
    {
        playerControl.rigidbody2D.drag = plagueAmount;
    }

	// Update is called once per frame
	void Update () {
		RegenBar();
		//UpdateHealthBar();
		if(GetComponent<NetworkController>().isOwner)
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
			if (currentStunMeter >= maxStun){

				Stun(); //this will only matter if called on the client which controls the player to be stunned
			}
		}
	}
	
	void FixedUpdate(){
     	//push the character sideways
		if(knockBackPending){
			//player should be in air by now, so disable movement
			playerControl.KnockBack();
			knockBackPending = false;
            
			//hitMarkSprites.enabled = false;
			//networkView.RPC ("VisualHitIndicator", RPCMode.Others, 0);
            
            rigidbody2D.velocity = new Vector2(0f, 0f);
			rigidbody2D.AddRelativeForce(velocityChange);
            //rigidbody2D.AddForce(sideForce);
		}
	}
	
	Vector2 upForce = new Vector2(0f, 200f);
	//Push the character up on this step
	void BeginKnockBack(float flip){
        rigidbody2D.AddRelativeForce(upForce);
        sideForce = new Vector2(7f * flip, 7f);
        velocityChange = (sideForce * rigidbody2D.mass / Time.fixedDeltaTime);
        knockBackPending = true;
	}
	
	void TakeHitOffline(bool fromLeftSide ){
		if(playerControl.knockedBack ){
			return;
		}
		HitFeedback ();

		float flip = 1f;
		if(!fromLeftSide)
			flip = -1f;
		
		BeginKnockBack(flip);

	}

	void TakeHit(bool fromLeftSide, NetworkPlayer attacker){

		if(!GetComponent<NetworkController>().isOwner)
			return;

		//ignore hits if the player is already knocked back
		if(playerControl.knockedBack ){
			return;
		}
		
		//hitCount++;
		//This tells all other players to visually play a hit effect. 
		//Only the owner of the character does anything with physics or death.
		if(!playerControl.dead)
			GenerateEvent(PowerType.MELEE, TimeManager.instance.time, attacker);
		networkView.RPC ("VisualHitIndicator", RPCMode.Others);
		VisualHitIndicator();

		if(Analytics.Enabled){
			GA.API.Design.NewEvent("Melee Attack Hits", transform.position);
		}

		if(playerControl.deathOnHit){

			print("shattered");
			playerControl.Die();
			return;
		}

			float flip = 1f;
			if(!fromLeftSide)
				flip = -1f;
			
			BeginKnockBack(flip);
	
	}

	
	public void HitFeedback(){
//		audio.Play (); 

		if(playerControl.facingRight){

			punchParticles.transform.localScale = new Vector3(1,1,1);

		}else{
			punchParticles.transform.localScale = new Vector3(-1,1,1);
		}

		punchEffect.Play();
	}
	
	
	[RPC]
	void VisualHitIndicator(){
		
		HitFeedback();

	}
		

	
	//allows other objects to appliy a specific amount of damage
	public void TakeDamage( float dmgTaken ){
		//Don't worry about lag compensation for now and just notify all other of the stun bar being raised.
		//TODO: Have stun bar values more synchronized, because bar may decay differently on other clients.
		networkView.RPC("NotifyDamageTaken", RPCMode.Others, dmgTaken);
		ApplyDamage(dmgTaken);
	}

	[RPC]
	void NotifyPlayerOfHit(bool fromLeftSide, NetworkPlayer attacker){
		TakeHit(fromLeftSide, attacker);
	}

	//Notify server that you hit a player (this can include server player)
	[RPC]
	void NotifyServerOfHit(bool fromLeftSide, NetworkPlayer hitPlayer, NetworkMessageInfo info){
		if(hitPlayer == Network.player){
			TakeHit(fromLeftSide, info.sender);
		}
		else{
			//RPC the player that got hit directly
			networkView.RPC("NotifyPlayerOfHit", hitPlayer,  fromLeftSide, info.sender);
		}
	}


	//This function tells the player that owns this character that he's been hit by you.
	public void AddHit( bool fromLeftSide){
			//Offline Melee
		if (playerControl.OFFLINE) {
						TakeHitOffline (fromLeftSide);
				} else {
						//Note, the following doesnt work(clients cant rpc each other directly)
						//networkView.RPC ("TakeHit", GetComponent<NetworkController>().theOwner, fromLeftSide); 
						//networkView.RPC ("TakeHit", RPCMode.Others, fromLeftSide);
						if (!playerControl.powerInvulnerable) {
								NetworkPlayer ownerOfPlayerClone = GetComponent<NetworkController> ().theOwner;
								if (Network.isServer) {
										//RPC player we hit directly
										networkView.RPC ("NotifyPlayerOfHit", ownerOfPlayerClone, fromLeftSide, Network.player);
								} else {
										//tell server we hit player
										networkView.RPC ("NotifyServerOfHit", RPCMode.Server, fromLeftSide, ownerOfPlayerClone);
								}

						}
				}
	}
	
	//checks if player is stun and then applies stunRecover
	private void CheckIfStunned(){
		if( playerControl.stunned == true ){
			playerControl.snared = true;
			TurnOnMashAlert();
			StunRecover();
			if( currentStunMeter <= 0 ){
				UnStun();
			}
		}
	}
	
	public void Stun(){
		playerControl.stunned = true;
		currentStunMeter = maxStun;
		playerControl.anim.SetTrigger("stunned");
	}
	
	public void Stun( float newRecoverRate ){

		playerControl.stunned = true;
		currentStunMeter = maxStun;
		recoverRate = newRecoverRate;
	}

    /*public void MeleeStun()
    {
        playerControl.meleeStunned = true;
        playerControl.snared = true;
        currentStunMeter = maxStun;
    }*/

	public void Frozen(){

		playerControl.stunned = true;
		currentStunMeter = maxStun;
		playerControl.anim.SetTrigger ("Frozen");
		recoverRate = 5.0f;
		playerControl.deathOnHit = true;

	}
	

	[RPC]
	void RemoveStunRemote(){
		UnStunSimple();
	}

	public void UnStunSimple(){
		recoverRate = 12.0f;
		playerControl.snared = false;
		//playerControl.meleeStunned = false;
		playerControl.stunned = false;
		TurnOffMashAlert();
		playerControl.anim.SetTrigger("unstunned");
		currentStunMeter = 0;
		playerControl.anim.enabled = true;
		playerControl.deathOnHit = false;
	}
	
	public void UnStun(){

		if( !playerControl.OFFLINE )
		networkView.RPC("RemoveStunRemote", RPCMode.Others);

		UnStunSimple();
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
		Destroy( MashIcon );
		Destroy( UI );
	}

	//Generates an event and calls function if one exists
	public void GenerateEvent(Power power){

		PlayerEvent playerEvent = null;
		
		if(power.spawnInfo != null){
			if(power.spawnInfo.type.IsPsuedoPower() || !power.spawnInfo.createEvents)
				return;
			if(power.spawnInfo.owner == null)
			{
				//no attacker.
				playerEvent = new PlayerEvent(power.spawnInfo.type, TimeManager.instance.time);
			}
			else 
			{
				playerEvent = new PlayerEvent(power.spawnInfo.type, 
				                              TimeManager.instance.time, power.spawnInfo.owner);
				playerEvent.Attacker = power.spawnInfo.owner;
			}

		}
		else{
			//Gotta be spikes
			playerEvent = new PlayerEvent(PowerType.SPIKES, TimeManager.instance.time);
		}
		
		if(eventAction != null){
			eventAction(networkController.theOwner, playerEvent);
		}
	}

	public void GenerateEvent(PowerType type, float time, NetworkPlayer attacker){
		PlayerEvent playerEvent = new PlayerEvent(type, time, attacker);
		if(eventAction != null){
			eventAction(networkController.theOwner, playerEvent);
		}
	}
} 
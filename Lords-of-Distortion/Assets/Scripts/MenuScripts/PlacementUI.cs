﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlacementUI : MonoBehaviour {

	public delegate void SpawnAction(PowerSpawn spawnInfo, GameObject ui);
	public event SpawnAction spawnNow;

	//The prefab for the UI elements in grid.
	public GameObject PowerBoard;
	public GameObject PowerSlot;

	public GameObject dottedLine;

	//Holds the entries for powers.
	public UIGrid TriggerGrid;

	public Dictionary<PowerType, Sprite> icons;
	string[] triggerKeys = new string[] {"1", "2", "3", "4", "5"};


	public Sprite glueSprite;
	public Sprite smokeSprite;
	public Sprite inkSprite;
	public Sprite windSprite;
	public Sprite transferSprite;
	public Sprite boulderSprite;
	public Sprite freezeSprite;
	public Sprite gateSprite;
	public Sprite plagueSprite;
	public Sprite electricSprite;
	public Sprite earthSprite;
	public Sprite blackholeSprite;

	private List<UIButton> buttons = new List<UIButton>();
	private Dictionary<PowerType, PowerBoard> boardsByType = new Dictionary<PowerType, PowerBoard>();
	private List<PowerBoard> fixedBoards = new List<PowerBoard>();

	Dictionary<PowerType, InventoryPower> inventoryPowers;
	Dictionary<GameObject, PowerSpawn> placedPowers = new Dictionary<GameObject, PowerSpawn>();


    public List<GameObject> allTrapsGO = new List<GameObject>();
    public List<GameObject> slots = new List<GameObject>();
    public List<GameObject> dummyInv = new List<GameObject>();

    // allTraps is the powers placed
	public List<PowerSpawn> allTraps = new List<PowerSpawn>();
	public Queue<PowerSpawn> delayedTraps = new Queue<PowerSpawn>(); //These are deleted when info is sent to server
	public List<PowerSpawn> activatedTraps = new List<PowerSpawn>(); //might not need this
	public Dictionary<int, PowerSpawn> spawnByID = new Dictionary<int, PowerSpawn>();


	PowerPrefabs powerPrefabs;

    private Camera cam;
    float timer = 0.0f;
    public UISprite deadLordBtnRed;
   
	bool powerButtonsEnabled = true;
	enum PlacementState{
		Default,
		MovingPower,
		ChangingDirection,
	}

	/*
	 * TODO: figure out a way to differentiate and/or transition between
	 * a) traps that are being placed (to do: disable animations from the start) highlight a certain color (your own?)
	 * b) traps that have been placed but not armed (keep them the same as a?)
	 * c) traps that have been armed - display cooldown on ui and remove special color
	 * d) traps that are real - DONE warning sign and enabling of animations
	 */

	//http://www.youtube.com/watch?v=eUFY8Zw0Bag
	public bool live = false;
	bool deadScreen = false;
	bool autoRefill = false;

	PlacementState state = PlacementState.Default;

	/*Variables used for keeping track of what we're placing*/
	PowerType activePowerType;
	GameObject activePower;
	GameObject dottedLineInstance;


	void Awake(){
		inventoryPowers = new Dictionary<PowerType, InventoryPower>();
		icons = new Dictionary<PowerType, Sprite>();

		/* Boring Initialization code for icons.*/
		icons.Add(PowerType.STICKY, glueSprite);
		icons.Add(PowerType.SMOKE, smokeSprite);
		icons.Add(PowerType.FIREBALL, inkSprite);
		icons.Add(PowerType.GRAVITY, windSprite);
		icons.Add(PowerType.EXPLOSIVE, transferSprite);
		icons.Add(PowerType.BOULDER, boulderSprite);
		icons.Add(PowerType.FREEZE, freezeSprite);
		icons.Add(PowerType.GATE, gateSprite);
		icons.Add (PowerType.ELECTRIC, electricSprite);
		icons.Add (PowerType.PLAGUE, plagueSprite);
		icons.Add (PowerType.EARTH, earthSprite);
		icons.Add (PowerType.HOLE, blackholeSprite);
		/*Hard code some powers for now*/
		/*
         * draftedPowers.Add(PowerType.SMOKE, new InventoryPower(PowerType.SMOKE, 1, "Chalk Dust"));
		 * draftedPowers.Add(PowerType.GRAVITY, new InventoryPower(PowerType.GRAVITY, 1, "Pinwheel"));
		 * draftedPowers.Add(PowerType.FIREBALL, new InventoryPower(PowerType.FIREBALL, 1, "Ink Shot"));
		 * draftedPowers.Add(PowerType.EXPLOSIVE, new InventoryPower(PowerType.EXPLOSIVE, 1, "Transfer Bomb"));
         */

	}
	
	void Start(){
        cam = Camera.main;
	}

	//Pass in dependencies
	//TODO: check if initialize was called.
	public void Initialize(PowerPrefabs powerPrefabs){
		this.powerPrefabs = powerPrefabs;

		/*Turn available powers into empty boards depending on count. Also
		 puts in initial slots.*/
		/* Randomize some powers */
		PowerType powerNum1;
		PowerType powerNum2;
		powerNum1 = PowerTypeExtensions.RandomActivePower();
		powerNum2 = PowerTypeExtensions.RandomPassivePower();
		inventoryPowers.Add(powerNum1, new InventoryPower(powerNum1, false));
		inventoryPowers.Add(powerNum2, new InventoryPower(powerNum2, false));

		int i = 1;
		foreach(var inventoryPower in inventoryPowers){
			GameObject entry = NGUITools.AddChild(TriggerGrid.gameObject, PowerBoard);
			entry.GetComponent<PowerBoard>().index = i;
			fixedBoards.Add( entry.GetComponent<PowerBoard>());
			AddToInventory(inventoryPower.Value);
			i++;
		}
		TriggerGrid.Reposition();
	}

	//Instead of letting system to it randomly, pick 2 powers to start with
	public void Initialize(PowerPrefabs powerPrefabs, PowerType slotA, PowerType slotB){
		
	}

	//Finds first empty powerboard and insert new power.
	void AddToInventory(InventoryPower associatedPower){

		foreach(PowerBoard board in fixedBoards){

			if(board.currentPower == null){
				//add slot as child to empty board
				GameObject slot = NGUITools.AddChild(board.gameObject, PowerSlot);

				slot.GetComponent<PowerSlot>().Initialize(icons[associatedPower.type], associatedPower);
				slots.Add(slot);
				board.SetChild(slot.GetComponent<PowerSlot>());
				buttons.Add(slot.GetComponent<UIButton>());
				UIEventListener.Get(slot).onPress  += PowerButtonClick;

				PowerBoard boardReference = null;
				boardsByType.TryGetValue(associatedPower.type, out boardReference);
				if(boardReference == null){
					boardsByType.Add(associatedPower.type, board);
				}
				break;
			}
		}
	}

	//removes slot from board.
	public void RemoveFromInventory(PowerType type){
		GameObject slot = boardsByType[type].currentPower.gameObject;
		buttons.Remove(slot.GetComponent<UIButton>());
		boardsByType[type].RemoveChild();
		boardsByType.Remove(type);
		inventoryPowers.Remove(type);
		//Switch to random power.
		if(autoRefill){
			Resupply();
			//GridEnabled(false);
		}
	}

	//Enable text label and set a key
	void GiveTrigger(GameObject slot, int triggerKey){
		slot.GetComponent<PowerSlot>().keyText = triggerKey.ToString();
		slot.GetComponentInChildren<UILabel>().enabled = true;
		slot.GetComponentInChildren<UILabel>().text = triggerKey.ToString();
	}


	/// <summary>
	///Place powers instantly without using triggers. 
	/// </summary>
	public void SwitchToLive(bool infinitePowers){
		live = true;

		if(infinitePowers){

			//Only limit placement if the player is dead and has infinite powers.
			//deadScreen = true;
			autoRefill = true;
			//resupply twice
			Resupply();
			Resupply();

			/*Unlimited powers.
			foreach(var inventoryPower in inventoryPowers){
				inventoryPower.Value.quantity = int.MaxValue;
				inventoryPower.Value.infinite = true;
			}
			*/
		}
		//TODO: destroy untriggered trapss
		GridEnabled(true);
	}

	//Check which boards have something in them, and if they've been placed, enable triggers.
	public void ShowTriggersInitial(){
		state = PlacementState.Default;
		foreach(PowerBoard board in fixedBoards){
			if(board.currentPower.linkedSpawn != null){
				GiveTrigger(board.currentPower.gameObject, board.index);
				board.currentPower.EnableActivation();
			}
		}
	}

	bool reEnabledButtons = false;
	/* Takes care of mouse clicks on this screen, depending on what state we're in.*/
	void Update(){
        //UpdateTriggerColor();
        if (deadScreen)
        {
            timer -= Time.deltaTime;
            deadLordBtnRed.fillAmount = timer / 3.5f;
			if(timer <= 0f && !reEnabledButtons){
				GridEnabled(true);
				reEnabledButtons = true;
			}
        }

		//Advance armament time for delayed powers.
		for(int i = allTraps.Count - 1; i >= 0; i--){
			PowerSpawn spawn = allTraps[i];
			if(spawn.timerSet)
				spawn.ElapseTime(Time.deltaTime);
		}

		if(Input.GetMouseButtonDown(0)){

			switch(state){
			//Checks if we've clicked on a power that was already placed..
			case PlacementState.Default:
				//make sure not to allow player to move powers when dead or when under an active button
				if(!live && SelectExistingPower()){ 
					FollowMouse();
				}
				break;
			//Called when we're in the process of moving a power around.
			case PlacementState.MovingPower:
				//Put down power.
				PlacePower();
				break;

			//called when we're in the process of changing the direction of a power.
			case PlacementState.ChangingDirection:
				ChooseDirection();
                //activePower = null;
				break;
			}
		}
	}


	/*The button that was pressed is passed in as a GameObject*/
	public void PowerButtonClick(GameObject sender, bool isDown){
		if(isDown)
			return;

		PowerSlot activeInfo = sender.GetComponent<PowerSlot>();
		//Checks if we still have any left before doing anything.
		if (state != PlacementState.MovingPower && !live)
		{
			SpawnPowerVisual(activeInfo);
			FollowMouse();
			//removepowerfrom
		}
        else if (live)
        {
			if(!deadScreen){
				SpawnPowerVisual(activeInfo);
				FollowMouse();
			}
            // Uncomment the timer to set restrictions on how often players place powers while dead.
			else if(timer <= 0.0f){


				SpawnPowerVisual(activeInfo);
				FollowMouse();
			}
        }
	}

	//Do a raycast to determine if we've clicked on a power on screen.
	private bool SelectExistingPower(){
		//TODO: only hit things in the powers layer
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.up);
		if(hit.collider != null){
			if(hit.collider.tag.Equals("UIPower")){
				activePower = hit.transform.gameObject;
				PowerSpawn spawn= null;
				placedPowers.TryGetValue(activePower, out spawn);
				if(activePower == null || spawn == null)
					return false;
				activePowerType = spawn.type;
				return true;
			}
		}
		return false;
	}

    private void ManipulateActivePower(GameObject activePower)
    {
        if (activePowerType == PowerType.GRAVITY)
        {
            Destroy(activePower.GetComponent<GravityFieldMover>());
        }
        Destroy(activePower.GetComponent<Power>());

        Destroy(activePower.rigidbody2D);
        activePower.GetComponent<Collider2D>().isTrigger = true;
        activePower.tag = "UIPower";

        foreach (Transform child in activePower.transform)
        {
            if(child.GetComponent<Power>() != null)
                Destroy(child.GetComponent<Power>());
            if (child.GetComponent<Collider2D>() != null)
                Destroy(child.GetComponent<Collider2D>());
            if (child.GetComponent<BoxCollider2D>() != null)
                Destroy(child.GetComponent<BoxCollider2D>());
            if (child.GetComponent<CircleCollider2D>() != null)
                Destroy(child.GetComponent<CircleCollider2D>());
        }

        Color uiColor = new Color(.5f, 0f, 0f);
        uiColor.a = .6f;
        activePower.renderer.material.color = uiColor;
        ChangeParticleColor(activePower, uiColor);
    }

	/*Take a power prefab and strip it down to the visuals. .*/
	private void SpawnPowerVisual(PowerSlot info){

		//remove 1 from quanitity, disable button if == 0
			//info.GetComponent<UIButton>().isEnabled = false;

		//Remove button event listener, (so that we can use button as a trigger later)
		if(!deadScreen)
			UIEventListener.Get(info.gameObject).onPress  -= PowerButtonClick;


		activePowerType = info.associatedPower.type;

		activePower = Instantiate (powerPrefabs.list[(int)activePowerType],
		                           info.transform.position, Quaternion.identity) as GameObject;

        ManipulateActivePower(activePower);

		//power.GetComponent<ParticleSystem>().startColor;
	}

	//Adds a mouseFollower to the current power.
	private void FollowMouse(){
		state = PlacementState.MovingPower;
		activePower.AddComponent<MouseFollow>();
        activePower.GetComponent<MouseFollow>().camera = cam;
		/*Disable all other buttons while placing power*/
		if(deadScreen){
			timer = 3.5f;
			reEnabledButtons = false;
		}
		GridEnabled(false);
	}

	private void LivePlacement(PowerSpawn spawn){
		print ("disable triggering until time is up");
		spawn.SetTimer(3f); //start armament time
        
        //TODO: start radial cooldown on actives
		PowerBoard relevantBoard = boardsByType[spawn.type];
		PowerSlot slotFromBoard = relevantBoard.currentPower;
		//Give button a trigger (inital color)
		GiveTrigger(slotFromBoard.gameObject, relevantBoard.index);
		//dont immediately enable key triggering
		slotFromBoard.UseTimer();
		spawn.timeUpEvent += PowerArmed;
	}
	
	void ChangeParticleColor(GameObject power, Color color){

		ParticleSystem particleSystem = power.GetComponent<ParticleSystem>();

		if(particleSystem != null)
		{
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
			
			particleSystem.GetParticles(particles);

			for(int i = 0; i < particles.Length; ++i)
			{
				particles[i].color = color;
				particleSystem.SetParticles(particles, particleSystem.particleCount);
			}

			particleSystem.startColor = color;
		}

        foreach(Transform child in power.transform)
        {
            if (child.GetComponent<ParticleSystem>() != null)
                child.particleSystem.startColor = color;
        }
	}

	private void KillMovement(GameObject power){
		//Pause all animations
		if(power.GetComponent<Animator>() != null){
			power.GetComponent<Animator>().enabled = false;
		}
		
		if(power.GetComponent<ParticleSystem>() != null)
		{
			//pause movement of system.
			power.GetComponent<ParticleSystem>().Pause();
		}
		
        foreach(Transform child in power.transform)
        {
            if (child.GetComponent<ParticleSystem>() != null)
                child.particleSystem.Pause();
        }
	}


	private void PlacementData(GameObject powerspawned){

		GA.API.Design.NewEvent(powerspawned.name + " Spawn", powerspawned.transform.position);

	}

	//Sets down the power and stores it as a power spawn. 
	private void PlacePower(){
        
		Destroy(activePower.GetComponent<MouseFollow>());
		PowerSpawn spawn = null;

		/*Either generate a new PowerSpawn or modify an existing one.*/
		if(placedPowers.ContainsKey(activePower)){
			placedPowers.TryGetValue(activePower, out spawn);
		}
		else {
			spawn = new PowerSpawn();
			spawn.type = activePowerType;
			placedPowers.Add(activePower, spawn);
			allTraps.Add(spawn);
			spawnByID.Add(spawn.GetLocalID(), spawn);

			//link associated spawn to ui script
			PowerBoard relevantBoard = boardsByType[spawn.type];
			PowerSlot slotFromBoard = relevantBoard.currentPower;
			//Remove one from quantity
			--slotFromBoard.associatedPower.quantity;
			slotFromBoard.SetSpawn(spawn);

			/*
			 * revert to no passives
			if(PowerSpawn.TypeIsPassive(spawn.type)){
				//spawn.SetTimer(0f);
				delayedTraps.Enqueue(spawn);
			}
			*/
		}

		spawn.position = activePower.transform.position;

		if(GameObject.Find("CollectData") != null){

			PlacementData(activePower);
			
		}

		KillMovement(activePower);


		//Either go back to default state or require setting a direction.
		if(activePowerType.TypeRequiresDirection()){
			state = PlacementState.ChangingDirection;
			dottedLineInstance = Instantiate(dottedLine, spawn.position, Quaternion.identity) as GameObject;
            activePower.AddComponent<powerRotate>();
		}
		else {
			//When we're placing powers mid game:
			if(live){
				if(deadScreen){
					Destroy(activePower);
					spawnNow(spawn, boardsByType[activePowerType].currentPower.gameObject);
				}
				else{
					LivePlacement(spawn);
				}
			}

			state = PlacementState.Default;
			if(!deadScreen || !live) //dont renable buttons if in dead screen unless timer is up
				GridEnabled(true);
		}

	}

	public void PowerArmed(PowerSpawn powerSpawn){

		foreach (var pair in placedPowers)
		{
			if (powerSpawn == pair.Value)
			{
				Color original = Color.white;
				original.a = .5f;
				pair.Key.renderer.material.color = original;
				ChangeParticleColor(pair.Key, original);
			}
		}
	}

	public void ColorizeAll(){
		foreach (var pair in placedPowers)
		{
			Color original = Color.white;
			original.a = .5f;
			pair.Key.renderer.material.color = original;
			ChangeParticleColor(pair.Key, original);
		}
	}

	///<summary>Calculates a direction vector from the current power to the mouse. Stores
	///it in existing PowerSpawn object.</summary>
	private void ChooseDirection(){
		PowerSpawn spawn = null;
		placedPowers.TryGetValue(activePower, out spawn);

		Vector3 mousePosition = Camera.main.ScreenToWorldPoint
			(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));

		Vector3 direction = Vector3.Normalize(mousePosition - spawn.position);
		direction.z = 0f;
		spawn.direction = direction;
		Destroy(dottedLineInstance);
        Destroy(activePower.GetComponent<powerRotate>());


		if(live){
			if(deadScreen){
				Destroy(activePower);
				spawnNow(spawn, boardsByType[activePowerType].currentPower.gameObject);
			}
			else{
				LivePlacement(spawn);
			}
		}
		//Return buttons to normal
		state = PlacementState.Default;
		if(!deadScreen || !live)
			GridEnabled(true);
	}


	//Enables or disables the entire grid of buttons.
	private void GridEnabled(bool state){
		powerButtonsEnabled = state;
        foreach(UIButton button in buttons){
			//if a power is out, dont re-enable its button (unless in activation mode)
			if(button.GetComponent<PowerSlot>().associatedPower.quantity > 0 || button.GetComponent<PowerSlot>().activationMode)
				button.isEnabled = state;
		}
	}

	//TODO: play an animation that tells players they can now use their powers.
	//This function pauses animations for powers and destroyes any unplaced instances.
	public void DisableEditing(){
		if(dottedLineInstance != null){
			Destroy(dottedLineInstance);
		}

		if(state != PlacementState.Default){
			if(state == PlacementState.MovingPower)
				PlacePower();
			if(state == PlacementState.ChangingDirection)
				ChooseDirection();
			state = PlacementState.Default;
		}

		//TODO: move this to a one by one basis when you spawn it
        foreach (var pair in placedPowers)
        {
			//Destroy(pair.Key);
            Destroy(pair.Key.collider2D);
		}
        
	}
	public void Disable(){
		GridEnabled(false);
		this.enabled = false;
	}

	public void Enable(){
		GridEnabled(true);
		this.enabled = true;
	}

	public bool CanResupply(){
		return inventoryPowers.Count < 2;
	}
	public void Resupply(){
        // Make sure players can only hold at most 2 powers. In their inventory and on the map.
		if(inventoryPowers.Count < 2){
			//avoid giving same power
			PowerType newPower = PowerType.UNDEFINED;
			do {
				newPower =  PowerTypeExtensions.RandomPower();
			} while (inventoryPowers.ContainsKey(newPower));

			InventoryPower freePower = new InventoryPower(newPower, false);

			inventoryPowers.Add(newPower, freePower);
			AddToInventory(freePower);
        }
	}

	//Destroy the associated UI power based solely on localID
	public void DestroyUIPower(Power power){
		DestroyUIPower(spawnByID[power.spawnInfo.GetLocalID()]);
	}

    public void DestroyUIPower(PowerSpawn spawn)
    {
		//print ("delete it");
		allTraps.Remove(spawn); //TODO: remove from grid
		GameObject key = null;
        foreach (var pair in placedPowers)
        {
            if (spawn == pair.Value)
            {
				spawnByID.Remove(spawn.GetLocalID());
                Destroy(pair.Key);
				key = pair.Key;
            }
        }
		if(key != null){
			placedPowers.Remove(key);
		}
    }


}

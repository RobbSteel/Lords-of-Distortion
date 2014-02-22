﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementUI : MonoBehaviour {

	public delegate void SpawnAction(PowerSpawn spawnInfo, GameObject ui);
	public event SpawnAction spawnNow;


	//The prefab for the UI elements in grid.
	public GameObject PowerBoard;
	public GameObject PowerSlot;

	public GameObject dottedLine;

	//Holds the entries for powers.
	public UIGrid InventoryGrid;
	public UIGrid TriggerGrid;

	public Dictionary<PowerType, Sprite> icons;
	string[] triggerKeys = new string[] {"1", "2", "3", "4", "5"};


	public Sprite glueSprite;
	public Sprite smokeSprite;
	public Sprite inkSprite;
	public Sprite windSprite;
	public Sprite transferSprite;

	private List<UIButton> buttons = new List<UIButton>();
	Dictionary<PowerType, InventoryPower> draftedPowers;
	Dictionary<GameObject, PowerSpawn> placedPowers = new Dictionary<GameObject, PowerSpawn>();

	public List<PowerSpawn> selectedTraps = new List<PowerSpawn>();
	public List<PowerSpawn> selectedTriggers = new List<PowerSpawn>();

	PowerPrefabs prefabs;

    private Camera cam;
    float timer = 0.0f;
    public UISprite deadLordBtnRed;
   
	bool powerButtonsEnabled = true;
	enum PlacementState{
		Default,
		MovingPower,
		ChangingDirection,
	}

	//http://www.youtube.com/watch?v=eUFY8Zw0Bag
	public bool live = false;


	PlacementState state = PlacementState.Default;

	/*Variables used for keeping track of what we're placing*/
	PowerType activePowerType;
	GameObject activePower;
	GameObject dottedLineInstance;

	void Awake(){
		draftedPowers = new Dictionary<PowerType, InventoryPower>();
		prefabs = GetComponent<PowerPrefabs>();
		icons = new Dictionary<PowerType, Sprite>();

		/* Boring Initialization code for icons.*/
		icons.Add(PowerType.STICKY, glueSprite);
		icons.Add(PowerType.SMOKE, smokeSprite);
		icons.Add(PowerType.FIREBALL, inkSprite);
		icons.Add(PowerType.GRAVITY, windSprite);
		icons.Add(PowerType.EXPLOSIVE, transferSprite);

		/*Hard code some powers for now*/
		draftedPowers.Add(PowerType.SMOKE, new InventoryPower(PowerType.SMOKE, 1, "Chalk Dust"));
		draftedPowers.Add(PowerType.GRAVITY, new InventoryPower(PowerType.GRAVITY, 1, "Pinwheel"));
		draftedPowers.Add(PowerType.FIREBALL, new InventoryPower(PowerType.FIREBALL, 1, "Ink Shot"));
		draftedPowers.Add(PowerType.EXPLOSIVE, new InventoryPower(PowerType.EXPLOSIVE, 1, "Transfer Bomb"));
	}
	
	void Start(){
        cam = Camera.main;

		/*Turn available powers into UI buttons.*/
		foreach(var inventoryPower in draftedPowers){

			//GameObject entry = Instantiate (PowerEntry, transform.position, Quaternion.identity) as GameObject;
			GameObject entry = NGUITools.AddChild(InventoryGrid.gameObject, PowerBoard);
			buttons.Add(entry.GetComponent<UIButton>());
			UIEventListener.Get(entry).onClick  += PowerButtonClick;
			PowerBoard info = entry.GetComponent<PowerBoard>();
			info.Initialize(inventoryPower.Value, icons[inventoryPower.Key]);
		}
		InventoryGrid.Reposition();
	}
	

	/// <summary>
	///Place powers instantly without using trigges. 
	/// </summary>
	public void SwitchToLive(){
		live = true;
		int i = 0;
		foreach(var inventoryPower in draftedPowers){
			//Unlimited powers.
			inventoryPower.Value.quantity = int.MaxValue;
		}
		GridEnabled(true);
	}

	//Called when we want to tween away our GUI.
	public void ShowTriggers(){
		state = PlacementState.Default;
		int i = 0;
		foreach(PowerSpawn spawn in selectedTriggers){
			GameObject slot = NGUITools.AddChild(TriggerGrid.gameObject, PowerSlot);
			Sprite sprite = null;
			icons.TryGetValue(spawn.type, out sprite);
			slot.GetComponent<PowerSlot>().Initialize(triggerKeys[i], sprite, spawn);
			i++;
		}
		TriggerGrid.Reposition();
	}


	/* Takes care of mouse clicks on this screen, depending on what state we're in.*/
	void Update(){

        if (live)
        {
            timer -= Time.deltaTime;
            deadLordBtnRed.fillAmount = timer / 3.5f;
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
				break;
			}
		}
	}


	/*The button that was pressed is passed in as a GameObject*/
	public void PowerButtonClick(GameObject sender){
		PowerBoard activeInfo = sender.GetComponent<PowerBoard>();
		//Checks if we still have any left before doing anything.
		if (activeInfo.associatedPower.quantity > 0 && state != PlacementState.MovingPower && !live)
		{
			SpawnPowerVisual(activeInfo);
			FollowMouse();
		}
        else if (live && timer <= 0.0f)
        {
            SpawnPowerVisual(activeInfo);
            FollowMouse();
        }
	}

	//Do a raycast to determine if we've clicked on a power on screen.
	private bool SelectExistingPower(){
		//TODO: only hit things in the powers layer
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.up);
		if(hit != null){
			if(hit.collider.tag.Equals("Power")){
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

	/*Take a power prefab and strip it down to the visuals. .*/
	private void SpawnPowerVisual(PowerBoard info){

		//remove 1 from quanitity, disable button if == 0
		if(--info.associatedPower.quantity <= 0)
			info.GetComponent<UIButton>().isEnabled = false;

		activePowerType = info.associatedPower.type;

		activePower = Instantiate (prefabs.list[(int)activePowerType],
		                           info.transform.position, Quaternion.identity) as GameObject;
		Destroy (activePower.GetComponent<Power>());
		Destroy (activePower.rigidbody2D);
		

		if(activePower.GetComponent<Animator>() != null){
			activePower.GetComponent<Animator>().enabled = false;
		}
	}

	//Adds a mouseFollower to the current power.
	private void FollowMouse(){
		state = PlacementState.MovingPower;
		activePower.AddComponent<MouseFollow>();
        activePower.GetComponent<MouseFollow>().camera = cam;
		/*Disable all other buttons while placing power*/
		GridEnabled(false);
	}

	//Sets down the power and stores it as a power spawn. 

	private void PlacePower(){
        timer = 3.5f;
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
			//TODO: Check if power is triggered or a trap.
			selectedTriggers.Add(spawn);
		}
		spawn.position = activePower.transform.position;

		//Either go back to default state or require setting a direction.
		if(PowerSpawn.TypeRequiresDirection(activePowerType)){
			state = PlacementState.ChangingDirection;
			dottedLineInstance = Instantiate(dottedLine, spawn.position, Quaternion.identity) as GameObject;
            activePower.AddComponent<powerRotate>();
		}
		else {

			if(live){
				Destroy(activePower);
				spawnNow(spawn, gameObject);
			}
            activePower = null;

			state = PlacementState.Default;
			GridEnabled(true);
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
			Destroy(activePower);
			activePower = null;
			spawnNow(spawn, gameObject);
		}

		//Return buttons to normal
		state = PlacementState.Default;
		GridEnabled(true);
	}


	//Enables or disables the entire grid of buttons.
	private void GridEnabled(bool state){
		powerButtonsEnabled = state;
        foreach(UIButton button in buttons){
			//if a power is out, dont re-enable it button
			if(button.GetComponent<PowerBoard>().associatedPower.quantity > 0)
				button.isEnabled = state;
		}        
	}

	public void DestroyPowers(){
		if(dottedLineInstance != null){
			Destroy(dottedLineInstance);
		}

		if(activePower != null){
			Destroy(activePower);
		}

        foreach (var pair in placedPowers)
        {
			//Destroy(pair.Key);
            Debug.Log(pair);
            Color color = pair.Key.renderer.material.color;
            color.a = 0.5f;
            pair.Key.renderer.material.color = color;
            if(pair.Key.GetComponent<ParticleSystem>() != null)
            {
                Color particleColor = pair.Key.GetComponent<ParticleSystem>().startColor;
                particleColor.a = 0.70f;
                pair.Key.GetComponent<ParticleSystem>().startColor = particleColor;
				//pause movement of system.
				pair.Key.GetComponent<ParticleSystem>().Pause();
			}

			if(pair.Key.GetComponentInChildren<ParticleSystem>() != null){
				print ("Particle Attempt");
				Color particleColor = pair.Key.GetComponentInChildren<ParticleSystem>().startColor;
				particleColor.a = 0.70f;
				pair.Key.GetComponentInChildren<ParticleSystem>().startColor = particleColor;
				//pause movement of system.
				pair.Key.GetComponentInChildren<ParticleSystem>().Pause();

			}

            Destroy(pair.Key.collider2D);
		}
        
	}

    public void DestroyAlphaPower(PowerSpawn spawn)
    {
        //PowerSpawn spawn = null;
        //placedPowers.TryGetValue(power, out spawn);
        //placedPowers.Remove(spawn);
        foreach (var pair in placedPowers)
        {
            if (spawn == pair.Value)
            {
                Destroy(pair.Key);
            }
        }
    }
}


using UnityEngine;
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
	public Sprite boulderSprite;

	private List<UIButton> buttons = new List<UIButton>();
	Dictionary<PowerType, InventoryPower> draftedPowers;
	Dictionary<GameObject, PowerSpawn> placedPowers = new Dictionary<GameObject, PowerSpawn>();



    public List<GameObject> currentPowers = new List<GameObject>();
    public List<GameObject> dummyInv = new List<GameObject>();

    // allTraps is the powers placed
	public List<PowerSpawn> allTraps = new List<PowerSpawn>();
	public Queue<PowerSpawn> delayedTraps = new Queue<PowerSpawn>(); //These are deleted when info is sent to server
	public List<PowerSpawn> activatedTraps = new List<PowerSpawn>(); //might not need this
	public Dictionary<int, PowerSpawn> spawnByID = new Dictionary<int, PowerSpawn>();

    private PowerType powerNum1;
    private PowerType powerNum2;

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


	PlacementState state = PlacementState.Default;

	/*Variables used for keeping track of what we're placing*/
	PowerType activePowerType;
	GameObject activePower;
	GameObject dottedLineInstance;

	void Awake(){
		draftedPowers = new Dictionary<PowerType, InventoryPower>();
		prefabs = GetComponent<PowerPrefabs>();
		icons = new Dictionary<PowerType, Sprite>();

        powerNum1 = PowerSpawn.RandomActivePower();
        powerNum2 = PowerSpawn.RandomPassivePower();

		/* Boring Initialization code for icons.*/
		icons.Add(PowerType.STICKY, glueSprite);
		icons.Add(PowerType.SMOKE, smokeSprite);
		icons.Add(PowerType.FIREBALL, inkSprite);
		icons.Add(PowerType.GRAVITY, windSprite);
		icons.Add(PowerType.EXPLOSIVE, transferSprite);
		icons.Add(PowerType.BOULDER, boulderSprite);

		/*Hard code some powers for now*/
		/*
         * draftedPowers.Add(PowerType.SMOKE, new InventoryPower(PowerType.SMOKE, 1, "Chalk Dust"));
		 * draftedPowers.Add(PowerType.GRAVITY, new InventoryPower(PowerType.GRAVITY, 1, "Pinwheel"));
		 * draftedPowers.Add(PowerType.FIREBALL, new InventoryPower(PowerType.FIREBALL, 1, "Ink Shot"));
		 * draftedPowers.Add(PowerType.EXPLOSIVE, new InventoryPower(PowerType.EXPLOSIVE, 1, "Transfer Bomb"));
         */
        /* Randomize some powers */

        draftedPowers.Add(powerNum1, new InventoryPower(powerNum1, 1));
        draftedPowers.Add(powerNum2, new InventoryPower(powerNum2, 1));
	}
	
	void Start(){
        cam = Camera.main;

		/*Turn available powers into UI buttons.*/
		foreach(var inventoryPower in draftedPowers){

			//GameObject entry = Instantiate (PowerEntry, transform.position, Quaternion.identity) as GameObject;
			GameObject entry = NGUITools.AddChild(TriggerGrid.gameObject, PowerBoard);
			buttons.Add(entry.GetComponent<UIButton>());
			UIEventListener.Get(entry).onClick  += PowerButtonClick;
			PowerBoard info = entry.GetComponent<PowerBoard>();
			info.Initialize(inventoryPower.Value, icons[inventoryPower.Key]);
            currentPowers.Add(entry);
		}
		TriggerGrid.Reposition();
	}

    /// <summary>
    /// Destroy GameObjects in dummyInv. Removes used items from Grid.
    /// </summary>
    public void DestroyDummyInv()
    {
        foreach (GameObject g0 in dummyInv)
        {
            NGUITools.Destroy(g0);
        }
    }

    /// <summary>
    /// Enable or Disable GameObjects tied to the player's original powers. 
    /// </summary>
    public void ToggleStartPowers(bool enabled)
    {
        foreach (GameObject g0 in currentPowers)
        {
            g0.SetActive(enabled);
        }
    }

	/// <summary>
	///Place powers instantly without using triggers. 
	/// </summary>
	public void SwitchToLive(bool infinitePowers){
		live = true;

		if(infinitePowers){
			//Only limit placement if the player is dead and has infinite powers.
			deadScreen = true;
            
            //Destroy boards placed in inventory grid used as the offset.
            DestroyDummyInv();  

            //Re-activate original powers for use.
            ToggleStartPowers(true);

			foreach(var inventoryPower in draftedPowers){
				//Unlimited powers.
				inventoryPower.Value.quantity = int.MaxValue;
			}
		}

		GridEnabled(true);
	}

	//Called when we want to tween away our GUI.
	public void ShowTriggers(){
		state = PlacementState.Default;
		int i = 0;

        //Deactivate current InventoryGrid so the icons disappear. Re-enable upon death.
        ToggleStartPowers(false);

        DestroyDummyInv();

        //Add powers to InventoryGrid that players can use to spawn powers mid game
        foreach (var inventoryPower in draftedPowers)
        {
            InventoryPower inv;
            draftedPowers.TryGetValue(inventoryPower.Key, out inv);
            if (inv.quantity > 0)
            {
                GameObject entry = NGUITools.AddChild(TriggerGrid.gameObject, PowerBoard);
                //buttons.Add(entry.GetComponent<UIButton>());
                UIEventListener.Get(entry).onClick += PowerButtonClick;
                PowerBoard info = entry.GetComponent<PowerBoard>();
                info.Initialize(inventoryPower.Value, icons[inventoryPower.Key]);
                dummyInv.Add(entry);
            }
        }
        TriggerGrid.Reposition();
        
        //Set Triggers in appropriate spot
        // Need something to prevent duplicates from happening
        foreach(PowerSpawn spawn in allTraps){
            Debug.Log("POWERSPAWN IN ALL TRAPS: " + spawn.type);
           
			GameObject slot = NGUITools.AddChild(TriggerGrid.gameObject, PowerSlot);
			Sprite sprite = null;
			icons.TryGetValue(spawn.type, out sprite);
			slot.GetComponent<PowerSlot>().Initialize(triggerKeys[i], sprite, spawn);
            slot.GetComponentInChildren<UILabel>().enabled = true;
            slot.GetComponentInChildren<UILabel>().text = (i+1).ToString();
            slot.GetComponentInChildren<UIStretch>().container = slot;
            slot.GetComponentInChildren<UIAnchor>().container = slot;
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
	public void PowerButtonClick(GameObject sender){
		PowerBoard activeInfo = sender.GetComponent<PowerBoard>();
		//Checks if we still have any left before doing anything.
		if (activeInfo.associatedPower.quantity > 0 && state != PlacementState.MovingPower && !live)
		{
			SpawnPowerVisual(activeInfo);
			FollowMouse();
		}
        else if (live)
        {
			if(!deadScreen){
				SpawnPowerVisual(activeInfo);
				FollowMouse();
			}
			else{// if(timer <= 0.0f){
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
		activePower.GetComponent<Collider2D>().isTrigger = true;
		activePower.tag = "UIPower";


		Color uiColor = Color.magenta;
		uiColor.a = .4f;
		activePower.renderer.material.color = uiColor;
		ChangeParticleColor(activePower, uiColor);

		//power.GetComponent<ParticleSystem>().startColor;
		
	}

	//Adds a mouseFollower to the current power.
	private void FollowMouse(){
		state = PlacementState.MovingPower;
		activePower.AddComponent<MouseFollow>();
        activePower.GetComponent<MouseFollow>().camera = cam;
		/*Disable all other buttons while placing power*/
		GridEnabled(false);
	}

	private void LivePlacement(PowerSpawn spawn){

		
		spawn.SetTimer(3f); //start armament time
		//TODO: start radial cooldown on actives
		/*
		 * switch back to all triggers.
		if(PowerSpawn.TypeIsPassive(spawn.type)){

			print ("add timed spawn locally and remotely");
			spawn.spawnTime = 3f; //spawn real trap 3 seconds later.
			//TODO: Instead of destroying, switch to live color. Destroy when someone sets off
			//spawn.timeUpEvent += DestroyUIPower; 
		}
		*/
	//	else{
			print ("disable triggering until time is up");
			//Do this for now:
			ShowTriggers();
			//activatedTraps.Add(spawn);
		//}
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


		if(power.GetComponentInChildren<ParticleSystem>() != null){
			power.GetComponentInChildren<ParticleSystem>().startColor = color;
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
		
		if(power.GetComponentInChildren<ParticleSystem>() != null){
			//print ("Particle Attempt");
			//pause movement of system.
			power.GetComponentInChildren<ParticleSystem>().Pause();
		}
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
			allTraps.Add(spawn);
			spawnByID.Add(spawn.GetLocalID(), spawn);
			/*
			 * revert to no passives
			if(PowerSpawn.TypeIsPassive(spawn.type)){
				//spawn.SetTimer(0f);
				delayedTraps.Enqueue(spawn);
			}
			*/
		}

		spawn.position = activePower.transform.position;

		KillMovement(activePower);


		//Either go back to default state or require setting a direction.
		if(PowerSpawn.TypeRequiresDirection(activePowerType)){
			state = PlacementState.ChangingDirection;
			dottedLineInstance = Instantiate(dottedLine, spawn.position, Quaternion.identity) as GameObject;
            activePower.AddComponent<powerRotate>();
		}
		else {
			//When we're placing powers mid game:
			if(live){
				if(deadScreen){
					Destroy(activePower);
					spawnNow(spawn, gameObject);
				}
				else
					LivePlacement(spawn);
			}

			state = PlacementState.Default;
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
				spawnNow(spawn, gameObject);
			}
			else
				LivePlacement(spawn);
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

	//TODO: play an animation that tells players they can now use their powers.
	//This function pauses animations for  powers and destroyes any unplaced instances.
	public void DestroyPowers(){
		if(dottedLineInstance != null){
			Destroy(dottedLineInstance);
		}

		if(state != PlacementState.Default){
			Destroy(activePower);
			state = PlacementState.Default;
		}

		//TODO: move this to a one by one basis when you spawn it
        foreach (var pair in placedPowers)
        {
			//Destroy(pair.Key);
            Debug.Log(pair);
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

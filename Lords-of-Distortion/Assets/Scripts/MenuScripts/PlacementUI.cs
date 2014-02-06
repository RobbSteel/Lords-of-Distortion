using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementUI : MonoBehaviour {
	//The prefab for the UI elements in grid.
	public GameObject PowerEntry;
	//Holds the entries for powers.
	public UIGrid Grid;

	private List<UIButton> buttons = new List<UIButton>();
	Dictionary<PowerType, InventoryPower> draftedPowers;
	PowerPrefabs prefabs;


	enum PlacementState{
		Default,
		MovingPower,
		ChangingDirection,
	}
	PlacementState state = PlacementState.Default;


	/*Variables used for keeping track of what we're placing*/
	PowerType activePowerType;
	GameObject activePower;


	void Awake(){
		draftedPowers = new Dictionary<PowerType, InventoryPower>();
		prefabs = GetComponent<PowerPrefabs>();

		/*Hard code some powers for now*/
		draftedPowers.Add(PowerType.SMOKE, new InventoryPower(PowerType.SMOKE, 1, "Smoke Bomb"));
		draftedPowers.Add(PowerType.GRAVITY, new InventoryPower(PowerType.GRAVITY, 2, "Gravity Field"));
		draftedPowers.Add(PowerType.FIREBALL, new InventoryPower(PowerType.FIREBALL, 1, "Fireball"));
	}

	void Start(){
		/*Turn available powers into UI elements buttons for now.*/
		foreach(var inventoryPower in draftedPowers){
			//GameObject entry = Instantiate (PowerEntry, transform.position, Quaternion.identity) as GameObject;
			GameObject entry = NGUITools.AddChild(Grid.gameObject, PowerEntry);
			buttons.Add(entry.GetComponent<UIButton>());
			UIEventListener.Get(entry).onClick  += PowerButtonClick;
			ButtonInfo info = entry.GetComponent<ButtonInfo>();
			info.Initialize(inventoryPower.Value);
		}
		Grid.Reposition();
	}

	void Update(){
		if(Input.GetMouseButtonDown(0)){
			switch(state){

			case PlacementState.MovingPower:
				//Put down power.
				PlacePower();
				break;
			}
		}
	}

	/*The button that was pressed is passed in as a GameObject*/
	public void PowerButtonClick(GameObject sender){
		ButtonInfo info = sender.GetComponent<ButtonInfo>();
		print(info.associatedPower.name);
		if(info.associatedPower.quantity > 0){
			SpawnPowerVisual(info);
			FollowMouse();
		}

		//sender.GetComponent<UIButton>().isEnabled = false;

	}

	/*Take a power prefab and strip it down to the visuals. Additionally, disable buttons.*/
	private void SpawnPowerVisual(ButtonInfo info){

		info.associatedPower.quantity--;
		info.UpdateQuantity();
		activePowerType = info.associatedPower.type;

		activePower = Instantiate (prefabs.list[(int)activePowerType],
		                           info.transform.position, Quaternion.identity) as GameObject;
		Destroy (activePower.GetComponent<Power>());
		/*Disable all other buttons while placing power*/
		GridEnabled(false);

	}

	private void FollowMouse(){
		state = PlacementState.MovingPower;
		activePower.AddComponent<MouseFollow>();
	}

	private void PlacePower(){
		Destroy(activePower.GetComponent<MouseFollow>());
		state = PlacementState.Default;
		GridEnabled(true);
	}

	private void GridEnabled(bool state){
		foreach(UIButton button in buttons){
			button.isEnabled = state;
		}
	}
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Priority_Queue;

public class OfflineArena : MonoBehaviour {

	public GameObject PlacementUIPrefab;
	public GameObject alertSymbolPrefab;

	private PlacementUI placementUI;
	private PowerPrefabs powerPrefabs;


	void Awake(){
		powerPrefabs = GetComponent<PowerPrefabs>();
		GameObject placementRoot = Instantiate(PlacementUIPrefab, PlacementUIPrefab.transform.position, 
		                                       Quaternion.identity) as GameObject;
		placementUI = placementRoot.GetComponent<PlacementUI>();
		placementUI.Initialize(powerPrefabs);
	}

	void Start(){
		placementUI.SwitchToLive(false);
	}

	void OnEnable(){
		PowerSlot.powerKey += SpawnTriggerPower;
		placementUI.spawnNow += SpawnTriggerPower;
	}


	void OnDisable(){
		PowerSlot.powerKey -= SpawnTriggerPower;
		placementUI.spawnNow -= SpawnTriggerPower;
	}



	void Update(){

	}

	void SpawnPowerLocally(int type, Vector3 position, Vector3 direction, NetworkViewID optionalViewID){
		PowerSpawn requestedSpawn = new PowerSpawn();
		requestedSpawn.type = (PowerType)type;
		requestedSpawn.position = position;
		requestedSpawn.direction = direction;
		SpawnPowerLocally(requestedSpawn, optionalViewID);
	}
	
	//The function that actually starts the coroutine for spawning powers.
	void SpawnPowerLocally(PowerSpawn spawn, NetworkViewID optionalViewID){
		StartCoroutine(YieldThenPower(spawn, optionalViewID));
	}

	IEnumerator YieldThenPower(PowerSpawn spawn, NetworkViewID optionalViewID)
	{
		GameObject instantiatedSymbol = (GameObject)Instantiate(alertSymbolPrefab, spawn.position, Quaternion.identity);
		yield return new WaitForSeconds(0.7f);
		Destroy(instantiatedSymbol);
		GameObject power =  Instantiate (powerPrefabs.list[(int)spawn.type], spawn.position, Quaternion.identity) as GameObject;;
		power.GetComponent<Power>().spawnInfo = spawn;
		//If the networkview id is specified, apply it to the networkview of the new power
		if(!Equals(optionalViewID, default(NetworkViewID))){
			power.GetComponent<NetworkView>().viewID = optionalViewID;
		}
	}

	private void SpawnTriggerPower(PowerSpawn spawn, GameObject uiElement){

		if (placementUI.allTraps.Contains(spawn))
		{
			//unitiliazed
			NetworkViewID newViewID = default(NetworkViewID);
			if(spawn.type == PowerType.EXPLOSIVE || spawn.type == PowerType.FIREBALL || spawn.type == PowerType.FREEZE){
				//Needs a viewID so that bombs can RPC each other.
				newViewID = Network.AllocateViewID();
			}

			SpawnPowerLocally(spawn, newViewID);

			//Remove from your inventory and  disable button
			placementUI.DestroyUIPower(spawn);
			if(uiElement.GetComponent<PowerSlot>() != null){
				PowerSlot powerSlot = uiElement.GetComponent<PowerSlot>();
				powerSlot.enabled = false;

				//remove from grid if power is not infinite.
				if(!powerSlot.associatedPower.infinite)
					placementUI.RemoveFromInventory(powerSlot.associatedPower.type);
			}
		}
	}
}

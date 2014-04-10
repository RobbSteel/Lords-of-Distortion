using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Priority_Queue;

public class OfflineArena : MonoBehaviour {
	public GameObject PlacementUIPrefab;
	private PlacementUI placementUI;
	private PowerPrefabs powerPrefabs;


	void Awake(){

		powerPrefabs = GetComponent<PowerPrefabs>();
		GameObject placementRoot = Instantiate(PlacementUIPrefab, PlacementUIPrefab.transform.position, 
		                                       Quaternion.identity) as GameObject;
		placementUI = placementRoot.GetComponent<PlacementUI>();
		placementUI.Initialize(powerPrefabs);

	}

	void Update(){

	}
}

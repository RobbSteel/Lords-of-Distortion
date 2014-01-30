using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LordSpawnManager : MonoBehaviour{



	void Start(){

		UIEventListener.Get(GameObject.Find("FireballButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("GravityFieldButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("SmokeBombButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("StickyTrapButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("FinishButton")).onClick += FinalizePowers;

		placedObjects = new Dictionary<PowerSpawn.PowerType, GameObject>();
		powerSpawns = new Dictionary<PowerSpawn.PowerType, PowerSpawn>();
	}


	void InitializePower(GameObject button){
		
		//var UIButton 
		//UIButton

		UILabel powerlabel = button.GetComponentInChildren<UILabel>();
		var powertype = powerlabel.text;
		print (powertype);


	if(!powerset){
		PowerSpawn spawn = new PowerSpawn();
		if(powertype == "Fireball"){
			spawn.type = PowerSpawn.PowerType.FIREBALL;
			
		}

		if(powertype == "Sticky Trap"){
			spawn.type = PowerSpawn.PowerType.STICKY;
		}

		if(powertype == "Gravity"){
			spawn.type = PowerSpawn.PowerType.GRAVITY;
		}

		if(powertype == "Smoke Bomb"){
			spawn.type = PowerSpawn.PowerType.SMOKE;
			
		}
			currPowerType = spawn.type;
			powerset = true;
			powerSpawns.Add(spawn.type, spawn);
	}


	}

	//For every placed object, retrieve key, retrieve value from power spawns using that key, set time
	//TODO: this button should really only disable further changes in UI.
	public void FinalizePowers(GameObject button){

		foreach(var pair in placedObjects){
			PowerSpawn.PowerType type = pair.Key;
			PowerSpawn spawn = null;
			powerSpawns.TryGetValue(type, out spawn);

			spawn.position = pair.Value.transform.position;

			//Instead of checking for what type something is before setting the time, we should have UI ids
			//so that no matter what power was in that button , the id would be used to determine what time
			//to use for which slot.
			switch (type) {
			case PowerSpawn.PowerType.FIREBALL:
				spawn.spawnTime = float.Parse(firetime.GetComponent<UIInput>().value);
				break;
			case PowerSpawn.PowerType.GRAVITY:
				spawn.spawnTime = float.Parse(gravitytime.GetComponent<UIInput>().value);
				break;
			case PowerSpawn.PowerType.SMOKE:
				spawn.spawnTime = float.Parse(smoketime.GetComponent<UIInput>().value);
				break;
			case PowerSpawn.PowerType.STICKY:
				spawn.spawnTime = float.Parse(stickytime.GetComponent<UIInput>().value);
				break;
			}
		}
		readyToSend = true;
	}

	void Update(){

		if(Input.GetMouseButtonDown(0) && powerset){
			var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
			mousePos.z = 0;
			var newpower = (Transform)Instantiate (powerlist[(int)currPowerType], mousePos, Quaternion.identity);
			
			placedObjects.Add(currPowerType, newpower.gameObject);
			powerset = false;
		}
	}


	public void DestroyPowers(){
		foreach(var pair in placedObjects){
			Destroy(pair.Value);
		}
	}



	public float spawnTime;
	public Vector3 position;
	public Vector3 direction;

	public Vector3 mousePos;
	public Transform[] powerlist; 
	public bool readyToSend = false;
	public bool powerset = false;
	public ArrayList powerinfo;
	public Dictionary<PowerSpawn.PowerType, GameObject> placedObjects;
	public Dictionary<PowerSpawn.PowerType, PowerSpawn> powerSpawns;
	public PowerSpawn.PowerType currPowerType;
	public List<Vector3> directions;

	public UIInput gravitytime;
	public UIInput firetime;
	public UIInput stickytime;
	public UIInput smoketime;

}
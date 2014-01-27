using UnityEngine;
using System.Collections;

public class LordSpawnManager : MonoBehaviour{



	void Start(){

		UIEventListener.Get(GameObject.Find("FireballButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("GravityFieldButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("SmokeBombButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("StickyTrapButton")).onClick += InitializePower;
	}


	void InitializePower(GameObject button){
		
		//var UIButton 
		//UIButton

		UILabel powerlabel = button.GetComponentInChildren<UILabel>();
		var powertype = powerlabel.text;
		print (powertype);
		PowerSpawn spawn = new PowerSpawn();

	if(!powerset){
		
		if(powertype == "Fireball"){
			spawn.type = PowerSpawn.PowerType.FIREBALL;
			currpower = powerlist[(int)spawn.type];
			powerset = true;
			
		}

		if(powertype == "Sticky Trap"){
			spawn.type = PowerSpawn.PowerType.STICKY;
			currpower = powerlist[(int)spawn.type];
			powerset = true;
			
		}

		if(powertype == "Gravity"){
			spawn.type = PowerSpawn.PowerType.GRAVITY;
			currpower = powerlist[(int)spawn.type];
			powerset = true;
			
		}

		if(powertype == "Smoke Bomb"){
			spawn.type = PowerSpawn.PowerType.SMOKE;
			currpower = powerlist[(int)spawn.type];
			powerset = true;
			
		}
	}


	}

	void Update(){

		if(Input.GetMouseButtonDown(0) && powerset){

		var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
		mousePos.z = 0;

		Instantiate (currpower, mousePos, Quaternion.identity);
		powerset = false;
		}
	}






	public float spawnTime;
	public Vector3 position;
	public Vector3 direction;
	public Transform currpower;
	public Vector3 mousePos;
	public Transform[] powerlist; 
	public bool powerset = false;
	public ArrayList powerinfo;



}
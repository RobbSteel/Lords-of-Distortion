﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LordSpawnManager : MonoBehaviour{



	void Start(){

		UIEventListener.Get(GameObject.Find("FireballButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("GravityFieldButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("SmokeBombButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("StickyTrapButton")).onClick += InitializePower;
		UIEventListener.Get(GameObject.Find("FinishButton")).onClick += SendPowers;
		powerspawned = new List<GameObject>();
	
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

	void SendPowers(GameObject button){


		for(int i = 0; i < powerspawned.Count; i++){

					var tempobj = powerspawned[i];
					locations.Add (tempobj.transform.position);
					powertype.Add (tempobj.name);
		
			}



	}

	void Update(){
	
	 sendgravity =float.Parse(gravitytime.GetComponent<UIInput>().value);
	 sendfire = float.Parse(firetime.GetComponent<UIInput>().value);
	 sendsticky = float.Parse(stickytime.GetComponent<UIInput>().value);
	 sendsmoke = float.Parse(smoketime.GetComponent<UIInput>().value);

		if(Input.GetMouseButtonDown(0) && powerset){

		var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
		mousePos.z = 0;

		var newpower = (GameObject)Instantiate (currpower, mousePos, Quaternion.identity);
		
		powerspawned.Add(newpower);
		powerset = false;
		}
	}


	void DestroyPowers(){

		for(int i = 0; i < powerspawned.Count; i++){

			Destroy(powerspawned[i]);

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
	public List<GameObject> powerspawned;

	public List<float> spawnTimes;
	public List<Vector3> locations;
	public List<Vector3> directions;
	public List<string> powertype;

	public UIInput gravitytime;
	public UIInput firetime;
	public UIInput stickytime;
	public UIInput smoketime;

	public float sendfire;
	public float sendsticky;
	public float sendgravity;
	public float sendsmoke;

}
﻿using UnityEngine;
using System.Collections;

public class StageSelect : MonoBehaviour {

	public GameObject difficulty;
	public GameObject pick;
	public string currentstage;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[RPC]
	void HostPickInfo(){

		DeleteInfo();
		StageInfo();
		MapDisplay();
		PickButton();
	}


void PickButton(){
	
		var picklabel = GameObject.Find("PickStage");
		var pickinfo = picklabel.GetComponent<AddStage>();
		pickinfo.stagename = gameObject.transform.name;
		pickinfo.stagedisplay = this.gameObject;
	}

	void StageInfo(){
		GameObject difflabel = (GameObject)Instantiate(difficulty, new Vector3(400, 0, 0), transform.rotation);
		difflabel.tag = "Display";
		difflabel.transform.parent = GameObject.Find("SelectUI(Clone)").transform;
		difflabel.transform.localScale = new Vector3(1, 1, 1);
		difflabel.transform.localPosition = new Vector2(325, -100);
		var difficultytext = difflabel.GetComponent<UILabel>();
		difficultytext.text = "Difficulty: Moderate";
	}

	void DeleteInfo(){

		if(GameObject.FindGameObjectWithTag("Display") != null){
			var deletion = GameObject.FindGameObjectsWithTag("Display");
				
				for(int i = 0; i < deletion.Length; i++){
					Destroy(deletion[i]);
				}
		}
	}

	void MapDisplay(){

		GameObject displaymap = (GameObject)Instantiate(this.gameObject, new Vector3(4.5f,2,0), transform.rotation);
		displaymap.transform.localScale *= 2.5f;
		var displayscript = displaymap.gameObject.GetComponent<StageSelect>();
		Destroy(displayscript);
		displaymap.tag = "Display";
	}

	void OnMouseDown(){
		if(Network.isServer){
		DeleteInfo();
		StageInfo();
		MapDisplay();
		PickButton();

		networkView.RPC("HostPickInfo", RPCMode.Others);
		}
	}
}

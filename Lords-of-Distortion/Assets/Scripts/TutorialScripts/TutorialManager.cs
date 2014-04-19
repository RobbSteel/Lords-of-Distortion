using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {
	
	private Camera mainCam;
	private GameObject player;
	private string[] Tutorial = new string[2]{"Tutorial-One", "Tutorial-Two"};
	private int currentLevel;
	public GameObject[] objectives;
	public int currentObjective;
	
	void Awake(){
		currentLevel = Application.loadedLevel;
		//make camera fade from black to clear
	}

	// Use this for initialization
	void Start () {
		currentObjective = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentObjective >= objectives.Length ){

			//make camera fade from clear to black
			loadNextLevel();
		}
	}

	private void incrementObjective(){
		if (objectives [currentObjective] == null) {
			currentObjective += 1;
		}
	}

	public void loadNextLevel(){
		currentLevel++;
		Application.LoadLevel(Tutorial[currentLevel]);
	}




}

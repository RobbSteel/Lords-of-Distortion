using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {
	
	GameObject mainscript;
	public MainGui playerscript;
	public PlayerServerInfo infoscript;
	
	// Update is called once per frame
	void Update () {
		//print(playerscript.playerName);
	}
	
	void OnPress(){
		Application.LoadLevel("Tutorial");
	}
}
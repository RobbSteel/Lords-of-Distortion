using UnityEngine;
using System.Collections;

public class CharSelected : MonoBehaviour {
	
	//public GameObject mainscript;
	public MainGui playerscript;
	public PlayerServerInfo infoscript;
	public UIButton button;



	// Use this for initialization
	void Start () {

	}
	
	void OnPress(bool isDown){
		if(isDown)
			return;

		/*Char1Selected = true;
		if(playerscript.playerName == "" || playerscript.playerName == "Player Name" || playerscript.gameName == "" || playerscript.gameName == "Server Name"){
			print("Nope");
		} else {
			infoscript.playername = playerscript.playerName;
			infoscript.servername = playerscript.gameName;
			infoscript.choice = "Host";
			Application.LoadLevel("LobbyArena");
			
		}*/
	}	
}

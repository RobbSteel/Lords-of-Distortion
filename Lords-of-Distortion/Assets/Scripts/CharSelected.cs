using UnityEngine;
using System.Collections;

public class CharSelected : MonoBehaviour {
	
	//public GameObject mainscript;
	public MainGui playerscript;
	public PlayerServerInfo infoscript;
	public UIButton button;
	public bool Char1Selected;
	// Use this for initialization
	void Start () {
		Char1Selected = false;

	}
	
	void OnPress(){
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

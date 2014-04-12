using UnityEngine;
using System.Collections;

public class Host : MonoBehaviour {

	//public GameObject mainscript;
	public MainGui playerscript;
	public PSInfo infoscript;


	// Use this for initialization
	void Start () {
	
	}

	void OnPress(){

		if(playerscript.playerName == "" || playerscript.playerName == "Player Name" || playerscript.gameName == "" || playerscript.gameName == "Server Name"){
			
			print("Nope");
			
		} else {
			infoscript.playername = playerscript.playerName;
			infoscript.servername = playerscript.gameName;
			infoscript.choice = "Host";
			Application.LoadLevel("LobbyArena");
			
		}
	}	
}

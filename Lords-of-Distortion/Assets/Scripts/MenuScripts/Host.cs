using UnityEngine;
using System.Collections;

public class Host : MonoBehaviour {

	//public GameObject mainscript;
	public MainGui playerscript;
	public PlayerServerInfo infoscript;


	// Use this for initialization
	void Start () {
	
	}

	void OnPress(){

		if(playerscript.playerName == "" || playerscript.playerName == "Player Name")
        {
			print("Nope");
		} 
        else
        {
			infoscript.localOptions.username = playerscript.playerName;
			infoscript.choice = "Host";
			Application.LoadLevel("LobbyArena");
			
		}
	}	
}

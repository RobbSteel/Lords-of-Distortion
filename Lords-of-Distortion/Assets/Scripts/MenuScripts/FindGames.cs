using UnityEngine;
using System.Collections;

public class FindGames : MonoBehaviour {

	GameObject mainscript;
	public MainGui playerscript;
	public PlayerServerInfo infoscript;
	
	// Update is called once per frame
	void Update () {
		//print(playerscript.playerName);
	}

	void OnPress(){

		if(playerscript.playerName == "" || playerscript.playerName == "Player Name"){

			print("Nope");

	} else {

			infoscript.localOptions.username = playerscript.playerName;
			infoscript.servername = playerscript.gameName;
			Application.LoadLevel("FindingGames");
		}
	}
}
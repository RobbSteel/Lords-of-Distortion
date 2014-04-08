using UnityEngine;
using System.Collections;

public class FindGames : MonoBehaviour {

	GameObject mainscript;
	public MainGui playerscript;
	public PSinfo infoscript;
	
	// Update is called once per frame
	void Update () {
		//print(playerscript.playerName);
	}

	void OnClick(){

		if(playerscript.playerName == "" || playerscript.playerName == "Player Name"){

			print("Nope");

	} else {

			infoscript.playername = playerscript.playerName;
			infoscript.servername = playerscript.gameName;
			Application.LoadLevel("FindingGames");
		}
	}
}
using UnityEngine;
using System.Collections;

public class FindGames : MonoBehaviour {

	GameObject mainscript;
	public MainGui playerscript;
	
	// Update is called once per frame
	void Update () {
		//print(playerscript.playerName);
	}

	void OnPress(){

		if(playerscript.playerName == "" || playerscript.playerName == "Player Name"){

			print("Nope");

		} else {

			PlayerServerInfo.instance.localOptions.username = playerscript.playerName;
			PlayerServerInfo.instance.servername = playerscript.gameName;
			Application.LoadLevel("FindingGames");
		}
	}
}
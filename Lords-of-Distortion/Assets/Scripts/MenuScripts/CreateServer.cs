using UnityEngine;
using System.Collections;

public class CreateServer : MonoBehaviour {

	//public GameObject mainscript;
	public MainGui playerscript;
    public GameObject playerName;

	// Use this for initialization
	void Start () {
		playerName.GetComponent<UILabel>().text = PlayerServerInfo.instance.localOptions.username;
	}

	void OnPress(){

		if(playerscript.gameName == "" || playerscript.gameName == "Server Name")
        {
			    print("Nope");
		}
        else
        {
			//infoscript.localOptions.username = playerscript.playerName;
			PlayerServerInfo.instance.servername = playerscript.gameName;
			PlayerServerInfo.instance.choice = "Host";
			Application.LoadLevel("LobbyArena");
		}
	}	
}

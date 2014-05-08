using UnityEngine;
using System.Collections;

public class CreateServer : MonoBehaviour {

	//public GameObject mainscript;
	public MainGui playerscript;
	public PlayerServerInfo infoscript;
    public GameObject playerName;

	// Use this for initialization
	void Start () {
        infoscript = GameObject.Find("PSInfo").GetComponent<PlayerServerInfo>();
        playerName.GetComponent<UILabel>().text = infoscript.localOptions.username;
	}

	void OnPress(){

		if(playerscript.gameName == "" || playerscript.gameName == "Server Name")
        {
			    print("Nope");
		}
        else
        {
			//infoscript.localOptions.username = playerscript.playerName;
			infoscript.servername = playerscript.gameName;
			infoscript.choice = "Host";
			Application.LoadLevel("LobbyArena");
		}
	}	
}

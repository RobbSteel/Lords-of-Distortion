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
			var liveslabel = GameObject.Find("enterLives");
			var lives = liveslabel.GetComponent<UILabel>().text;
			var convert = float.Parse(lives);
			if(convert == 0){
				convert = 1;
			}
			PlayerServerInfo.instance.lives = convert;
			Application.LoadLevel("LobbyArena");
		}
	}	
}

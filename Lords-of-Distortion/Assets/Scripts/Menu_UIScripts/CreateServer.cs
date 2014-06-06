using UnityEngine;
using System.Collections;

public class CreateServer : MonoBehaviour {

	//public GameObject mainscript;
	public MainGui playerscript;
    public GameObject playerName;
    public UILabel serverName;

	// Use this for initialization
	void Start () 
    {
		playerName.GetComponent<UILabel>().text = PlayerServerInfo.instance.localOptions.username;
	}

	void OnPress(){

        if (serverName.text != "" && serverName.text != "Server Name")
        {
            PlayerServerInfo.instance.servername = serverName.text;
            PlayerServerInfo.instance.choice = "Host";

            var liveslabel = GameObject.Find("enterLives");
            var lives = liveslabel.GetComponent<UILabel>().text;
            var convert = float.Parse(lives);
            if (convert == 0)
            {
                convert = 1;
            }

            var numStages = GameObject.Find("enterRounds").GetComponent<UILabel>().text;

            PlayerServerInfo.instance.livesPerRound = convert;
            PlayerServerInfo.instance.numStages = int.Parse(numStages);
            Application.LoadLevel("LobbyArena");
        }
	}	
}

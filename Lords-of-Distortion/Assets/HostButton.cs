using UnityEngine;
using System.Collections;

public class HostButton : MonoBehaviour {

    public MainGui playerscript;

    void OnPress()
    {

        if (playerscript.playerName == "" || playerscript.playerName == "Player Name")
        {
            print("Nope");
        }
        else
        {
			PlayerServerInfo.instance.localOptions.username = playerscript.playerName;
			PlayerServerInfo.instance.choice = "Host";
            Application.LoadLevel("HostMenu");
        }
    }	
}

using UnityEngine;
using System.Collections;

public class HostButton : MonoBehaviour {

    public MainGui playerscript;
    public PlayerServerInfo infoscript;

    void OnPress()
    {

        if (playerscript.playerName == "" || playerscript.playerName == "Player Name")
        {
            print("Nope");
        }
        else
        {
            infoscript.localOptions.username = playerscript.playerName;
            infoscript.choice = "Host";
            Application.LoadLevel("HostMenu");
        }
    }	
}

using UnityEngine;
using System.Collections;

public class HostButton : MonoBehaviour {

    public MainGui playerscript;
	public AudioClip buttonclick;
    void OnPress()
    {

        if (playerscript.playerName == "" || playerscript.playerName == "Player Name")
        {
            print("Nope");
        }
        else
        {
			audio.PlayOneShot(buttonclick);
			PlayerServerInfo.instance.localOptions.username = playerscript.playerName;
			PlayerServerInfo.instance.choice = "Host";
            Application.LoadLevel("HostMenu");
        }
    }	
}

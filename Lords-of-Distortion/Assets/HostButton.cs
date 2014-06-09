using UnityEngine;
using System.Collections;

public class HostButton : MonoBehaviour {

    public MainGui playerscript;
	public GameObject transitioner;
	Transitioner transition;
	public AudioClip pageturn;
	public AudioClip error;

    void OnPress()
    {

        if (playerscript.playerName == "" || playerscript.playerName == "Player Name")
        {
			audio.PlayOneShot(error, 0.2f);

        }
        else
        {
			transition = transitioner.GetComponent<Transitioner>();
			PlayerServerInfo.instance.localOptions.username = playerscript.playerName;
			PlayerServerInfo.instance.choice = "Host";
			audio.PlayOneShot(pageturn);
			transition.Flip("HostMenu", false);
        }
    }	
}

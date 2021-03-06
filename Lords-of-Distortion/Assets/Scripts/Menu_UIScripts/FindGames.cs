using UnityEngine;
using System.Collections;

public class FindGames : MonoBehaviour {

	GameObject mainscript;
	public MainGui playerscript;
	public GameObject transitioner;
	Transitioner transition;
	public AudioClip pageturn;
	public AudioClip error;

	// Update is called once per frame
	void Update () {
		//print(playerscript.playerName);
	}

	void OnPress(){

		if(playerscript.playerName == "" || playerscript.playerName == "Player Name"){

			audio.PlayOneShot(error, 0.2f);


		} else {
			transition = transitioner.GetComponent<Transitioner>();
			PlayerServerInfo.Instance.localOptions.username = playerscript.playerName;
			//PlayerServerInfo.instance.servername = playerscript.gameName;
			audio.PlayOneShot(pageturn);
			transition.Flip("FindingGames", false);
		}

	}
}
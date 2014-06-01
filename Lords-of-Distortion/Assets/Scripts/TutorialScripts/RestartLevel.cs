using UnityEngine;
using System.Collections;

public class RestartLevel : MonoBehaviour {

	public AudioClip buttonclick;

	void OnPress()
	{
		//restart level
		audio.PlayOneShot(buttonclick);
		Application.LoadLevel( Application.loadedLevel );
	}
}

using UnityEngine;
using System.Collections;

public class RestartLevel : MonoBehaviour {

	void OnPress()
	{
		//restart level
		Application.LoadLevel( Application.loadedLevel );
	}
}

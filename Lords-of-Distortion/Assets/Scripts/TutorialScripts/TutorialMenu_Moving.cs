using UnityEngine;
using System.Collections;

public class TutorialMenu_Moving : MonoBehaviour {

	private string movingTutorial = "Tutorial_moving";

	void OnPress()
	{
		Application.LoadLevel (movingTutorial);
	}

}

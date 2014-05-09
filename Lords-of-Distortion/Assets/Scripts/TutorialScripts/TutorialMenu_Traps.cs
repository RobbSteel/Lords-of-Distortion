using UnityEngine;
using System.Collections;

public class TutorialButton : MonoBehaviour {

	private string movingTutorial = "Tutorial_Moving_Run";

	void OnPress()
	{
		Application.LoadLevel (movingTutorial);
	}

}

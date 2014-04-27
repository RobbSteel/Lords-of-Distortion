using UnityEngine;
using System.Collections;

public class TutorialButton : MonoBehaviour {

	private string tutorialStartingStage = "Tutorial-One";

	void OnPress()
	{
		Application.LoadLevel (tutorialStartingStage);
	}

}

using UnityEngine;
using System.Collections;

public class TutorialMenu_Main : MonoBehaviour {

	private string tutorialMenu = "Tutorial_Menu";

	void OnPress()
	{
		Application.LoadLevel (tutorialMenu);
	}

}

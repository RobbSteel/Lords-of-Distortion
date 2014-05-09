using UnityEngine;
using System.Collections;

public class TutorialMenu_Back : MonoBehaviour {

	private string backToMainMenu = "MainMenu";

	void OnPress()
	{
		Application.LoadLevel (backToMainMenu);
	}

}

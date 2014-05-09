using UnityEngine;
using System.Collections;

public class TutorialMenu_Melee : MonoBehaviour {

	private string meleeTutorial = "Tutorial_Melee";

	void OnPress()
	{
		Application.LoadLevel (meleeTutorial);
	}

}

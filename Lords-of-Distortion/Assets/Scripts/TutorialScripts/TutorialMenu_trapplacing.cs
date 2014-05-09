using UnityEngine;
using System.Collections;

public class TutorialMenu_trapplacing : MonoBehaviour {

	private string trapplacingTutorial = "Tutorial_TrapPlacing";

	void OnPress()
	{
		Application.LoadLevel (trapplacingTutorial);
	}

}

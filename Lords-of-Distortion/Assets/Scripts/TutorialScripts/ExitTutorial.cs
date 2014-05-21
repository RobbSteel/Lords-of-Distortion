using UnityEngine;
using System.Collections;

public class ExitTutorial : MonoBehaviour {
	
	void OnPress()
	{
		//exit to tutorial menu
		Application.LoadLevel("Tutorial_Menu");
	}
}

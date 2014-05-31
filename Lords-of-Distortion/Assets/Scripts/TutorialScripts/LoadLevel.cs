using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

	[SerializeField]
	private string LevelName = "MainMenu";

	void OnPress(bool isDown)
	{
		if(!isDown)
		{
			Application.LoadLevel(LevelName);
		}
	}
}

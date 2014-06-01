using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

	[SerializeField]
	private string LevelName = "MainMenu";
	public AudioClip buttonhover;
	public AudioClip buttonclick;
	void OnPress(bool isDown)
	{

		if(!isDown)
		{
			audio.PlayOneShot (buttonclick);
			Application.LoadLevel(LevelName);

		}
	}
	void OnHover(bool isOver){
		if (isOver) {
			audio.PlayOneShot(buttonhover, 0.35f);
		}
	}
}

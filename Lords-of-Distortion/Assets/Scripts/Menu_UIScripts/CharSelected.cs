using UnityEngine;
using System.Collections;

public class CharSelected : MonoBehaviour {
	public PlayerServerInfo infoscript;
	public int characterNum;
	UIButtonColor buttcolor;
    CharSelectScript charscript;
	public AudioClip buttonhover;
	public AudioClip buttonclick;
	void Awake(){
        charscript = GameObject.Find("CharSelect").GetComponent<CharSelectScript>();
        infoscript = GameObject.Find("PSInfo").GetComponent<PlayerServerInfo>();
		buttcolor = GetComponent<UIButtonColor>();
	}

	void OnPress(bool isDown)
	{
		if (isDown) {
			audio.PlayOneShot(buttonclick,0.5f);
			return;
		}
		infoscript.localOptions.character = (Character)characterNum;
        charscript.UpdateBackgroundColor(characterNum);
	}
	void OnHover(bool isOver){
		if (isOver)
			audio.PlayOneShot(buttonhover, 0.25f);
	}
}

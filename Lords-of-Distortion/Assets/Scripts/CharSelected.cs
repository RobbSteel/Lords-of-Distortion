using UnityEngine;
using System.Collections;

public class CharSelected : MonoBehaviour {
	public PlayerServerInfo infoscript;
	public int characterNum;
	UIButtonColor buttcolor;
    CharSelectScript charscript;
	
	void Awake(){
        charscript = GameObject.Find("CharSelect").GetComponent<CharSelectScript>();
		buttcolor = GetComponent<UIButtonColor>();
	}

	void OnPress(bool isDown)
	{
		if (isDown) {
			return;
		}
		infoscript.localOptions.character = (PlayerOptions.Character)characterNum;
        charscript.UpdateBackgroundColor(characterNum);
	}
}

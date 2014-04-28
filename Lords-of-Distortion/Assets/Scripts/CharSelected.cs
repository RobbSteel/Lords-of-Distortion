using UnityEngine;
using System.Collections;

public class CharSelected : MonoBehaviour {
    public PlayerServerInfo infoscript;
    public int characterNum;
	UIButtonColor buttcolor;

	void Awake(){
		buttcolor = GetComponent<UIButtonColor>();
	}
	/*
	void Update(){
				if (infoscript.localOptions.character == (PlayerOptions.Character)characterNum) {
						buttcolor.defaultColor = Color.green;
						buttcolor.hover = Color.green;
						//buttcolor.pressed = Color.green;
				} else {
						buttcolor.defaultColor = Color.white;
						//buttcolor.hover = Color.white;
						//buttcolor.pressed = Color.white;
				}
		}
*/
	void OnPress(bool isDown)
    {

		if (isDown) {
			return;
		}
        infoscript.localOptions.character = (PlayerOptions.Character)characterNum;
	}
}

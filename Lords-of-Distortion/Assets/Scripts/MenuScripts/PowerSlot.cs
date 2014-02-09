using UnityEngine;
using System.Collections;

public class PowerSlot : MonoBehaviour {

	UILabel keyLabel;
	UISprite powerIcon;
	string keyText;

	public void Initialize(string key, Sprite sprite){
		keyText = key;
		keyLabel = transform.Find("TriggerKey").GetComponent<UILabel>();
		keyLabel.text = key;
		powerIcon = GetComponent<UISprite>();
		powerIcon.spriteName = sprite.name;
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Check for key press and call event .
	void Update () {
		if(Input.GetKeyDown(keyText)){
			print("Pressed " + keyText + ", try to spawn that power");
		}
	}

}

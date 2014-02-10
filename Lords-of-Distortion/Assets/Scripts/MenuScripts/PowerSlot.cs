using UnityEngine;
using System.Collections;

public class PowerSlot : MonoBehaviour {

	public PowerSpawn linkedSpawn;

	public delegate void KeyPress(PowerSpawn spawnInfo);
	public static event KeyPress powerKey;

	UILabel keyLabel;
	UISprite powerIcon;
	string keyText;

	public void Initialize(string key, Sprite sprite, PowerSpawn linkedSpawn){
		keyText = key;
		this.linkedSpawn = linkedSpawn;
		keyLabel = transform.Find("TriggerKey").GetComponent<UILabel>();
		keyLabel.text = key;
		powerIcon = GetComponent<UISprite>();
		powerIcon.spriteName = sprite.name;
	}


	// Check for key press and call event .
	void Update () {
		if(Input.GetKeyDown(keyText)){
			print("Pressed " + keyText + ", try to spawn that power");
			powerKey(linkedSpawn);
		}
	}
}

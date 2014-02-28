using UnityEngine;
using System.Collections;

public class PowerSlot : MonoBehaviour {

    const float FIGHT_COUNT_DOWN_TIME = 5f;

    public PowerSpawn linkedSpawn;

	public delegate void KeyPress(PowerSpawn spawnInfo, GameObject button);
	public static event KeyPress powerKey;
	UILabel keyLabel;
	UISprite powerIcon;
	string keyText;

	bool activationEnabled = true;

	public void Initialize(string key, Sprite sprite, PowerSpawn linkedSpawn){
		keyText = key;
		this.linkedSpawn = linkedSpawn;
		keyLabel = GameObject.Find("TriggerKey").GetComponent<UILabel>();
		//keyLabel.text = key;
		powerIcon = GetComponent<UISprite>();
		powerIcon.spriteName = sprite.name;
		if(linkedSpawn.timerSet){
			activationEnabled = false;
			linkedSpawn.timeUpEvent += EnableActivation;
		}
	}
	void EnableActivation(PowerSpawn spawn){
		activationEnabled = true;
	}

	// Check for key press and call event .
	void Update () {
        float currentTime = TimeManager.instance.time;
		if(activationEnabled && Input.GetKeyDown(keyText)){
			print("Pressed " + keyText + ", try to spawn that power");
			powerKey(linkedSpawn, this.gameObject);
		}
	}
}

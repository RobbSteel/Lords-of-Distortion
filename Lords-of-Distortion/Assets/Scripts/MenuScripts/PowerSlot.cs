using UnityEngine;
using System.Collections;

public class PowerSlot : MonoBehaviour {

    const float FIGHT_COUNT_DOWN_TIME = 5f;

    public PowerSpawn linkedSpawn;
    public bool wasSpawned = false;
	public delegate void KeyPress(PowerSpawn spawnInfo, GameObject button);
	public static event KeyPress powerKey;
	UILabel keyLabel;
	UI2DSprite powerIcon;
	public string keyText;
	bool activationEnabled = false;

	
	public InventoryPower associatedPower;


	public void Initialize(Sprite sprite, InventoryPower power){
		associatedPower = power;
        if(GameObject.Find("TriggerKey") != null)
        { 
		    keyLabel = GameObject.Find("TriggerKey").GetComponent<UILabel>();
        }
		powerIcon = GetComponent<UI2DSprite>();
		powerIcon.sprite2D = sprite;
	}

	public void SetSpawn(PowerSpawn linkedSpawn){
		this.linkedSpawn = linkedSpawn;
	}
	//make sure to call set spawn before this
	public void UseTimer(){
		if(linkedSpawn.timerSet){
			activationEnabled = false;
			linkedSpawn.timeUpEvent += EnableActivation;
		}
		else
			Debug.Log("Warning, you need to set a timer first.");
	}
	public void EnableActivation(){
		activationEnabled = true;
		keyLabel.color = new Color(0f, 1f, 0f, 1f);
	}

	void EnableActivation(PowerSpawn spawn){
		activationEnabled = true;
		keyLabel.color = new Color(0f, 1f, 0f, 1f);
	}

	// Check for key press and call event .
	void Update () {
		if(activationEnabled && Input.GetKeyDown(keyText)){
            wasSpawned = true;
			print("Pressed " + keyText + ", try to spawn that power");
			powerKey(linkedSpawn, this.gameObject);
		}
	}
}

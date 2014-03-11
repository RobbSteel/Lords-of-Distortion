using UnityEngine;
using System.Collections;

public class PowerSlot : MonoBehaviour {

    const float FIGHT_COUNT_DOWN_TIME = 5f;

    public PowerSpawn linkedSpawn;
    public bool wasSpawned = false;
	public delegate void KeyPress(PowerSpawn spawnInfo, GameObject button);
	public static event KeyPress powerKey;
	UILabel keyLabel;
	UIButton button;
	UI2DSprite powerIcon;
	public string keyText;
	bool activationEnabled = false;
	public bool activationMode = false;
	
	public InventoryPower associatedPower;

	void Start(){
		button = GetComponent<UIButton>();
	}


	public void Initialize(Sprite sprite, InventoryPower power){
		associatedPower = power;
        if(transform.Find("TriggerKey") != null)
        { 
			keyLabel = transform.Find("TriggerKey").GetComponent<UILabel>();
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
	//can be called from anywhere (no parameters)
	public void EnableActivation(){
		activationEnabled = true;
		activationMode = true;
		keyLabel.color = Color.green;
		button.isEnabled = true;
		UIEventListener.Get(this.gameObject).onClick += RequestSpawn;
	}
	//To be called by timer in powerspawn
	void EnableActivation(PowerSpawn spawn){
		EnableActivation();
	}

	void RequestSpawn(GameObject go){
		if(activationEnabled){
			wasSpawned = true;
			powerKey(linkedSpawn, this.gameObject);
		}
	}

	// Check for key press and call event .
	void Update () {
		if(activationEnabled && Input.GetKeyDown(keyText)){
			print("Pressed " + keyText + ", try to spawn that power");
			wasSpawned = true;
			powerKey(linkedSpawn, this.gameObject);
		}
	}
}

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

	static Color Green = new Color(0f, 230f, 0f);
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
		//powerIcon.width = powerIcon.calc
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
		keyLabel.color = Green;
		button.isEnabled = true;
		UIEventListener.Get(this.gameObject).onPress += RequestSpawn;
	}
	//To be called by timer in powerspawn
	void EnableActivation(PowerSpawn spawn){
		EnableActivation();
	}

	void RequestSpawn(GameObject go, bool isDown){
		if(isDown){
			return;
		}

		if(activationEnabled){
			wasSpawned = true;
			powerKey(linkedSpawn, this.gameObject);
		}
	}

	// Check for key press and call event .
	void Update () {
		if(Input.GetKeyDown(keyText)){
			if(activationEnabled){
				wasSpawned = true;
				powerKey(linkedSpawn, this.gameObject);
			}
			else {
				//simulate a button press
				if(button.isEnabled)
					UIEventListener.Get(gameObject).onPress(gameObject, false);
			}

		}
	}
}

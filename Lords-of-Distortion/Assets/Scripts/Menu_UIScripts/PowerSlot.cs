using UnityEngine;
using System.Collections;
using InControl;

public class PowerSlot : MonoBehaviour {
    public PowerSpawn linkedSpawn;
    public bool wasSpawned = false;
	public delegate void KeyPress(PowerSpawn spawnInfo, GameObject button);
	public static event KeyPress powerKey;
	UILabel keyLabel;
	UIButton button;
	UI2DSprite powerIcon;
	public string ActivationKey;
	bool activationEnabled = false;
	public bool activationMode = false;
	public InventoryPower associatedPower;

	static Color Green = new Color(0f, 230f, 0f);


	InputControlType activationButton;

	void Start()
	{
		button = GetComponent<UIButton>();
	}


	public void Initialize(Sprite sprite, InventoryPower power, int boardIndex){
		keyLabel = GetComponentInChildren<UILabel>();
		keyLabel.enabled = true;
		keyLabel.text = boardIndex.ToString();
		ActivationKey = boardIndex.ToString();

		associatedPower = power;
		powerIcon = GetComponent<UI2DSprite>();
		powerIcon.sprite2D = sprite;

		if(boardIndex == 1)
		{
			activationButton = InputControlType.RightBumper;
		}
		else 
		{
			activationButton = InputControlType.RightTrigger;
		}
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
		if(GameInput.instance.usingGamePad)
		{
			if(InputManager.ActiveDevice.GetControl(activationButton).WasPressed)
			{
				if(button.isEnabled)
					UIEventListener.Get(gameObject).onPress(gameObject, false);
			}
		}
		else if(Input.GetKeyDown(ActivationKey)){
			//simulate a button press
			if(button.isEnabled)
					UIEventListener.Get(gameObject).onPress(gameObject, false);
		}
	}
}

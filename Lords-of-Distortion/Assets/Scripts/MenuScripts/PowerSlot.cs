using UnityEngine;
using System.Collections;

public class PowerSlot : MonoBehaviour {

    const float FIGHT_COUNT_DOWN_TIME = 5f;

    public PowerSpawn linkedSpawn;

	public delegate void KeyPress(PowerSpawn spawnInfo);
	public static event KeyPress powerKey;
    private ArenaManager bt;
	UILabel keyLabel;
	UISprite powerIcon;
	string keyText;


    void Awake()
    {
        bt = GameObject.Find("ArenaOne").GetComponent<ArenaManager>();
    }

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
        float currentTime = TimeManager.instance.time;
		if(Input.GetKeyDown(keyText) && currentTime >= bt.getBeginTime() + FIGHT_COUNT_DOWN_TIME){
			print("Pressed " + keyText + ", try to spawn that power");
			powerKey(linkedSpawn);
			Vector3 offscreen = transform.position;
			offscreen.y -= 400f;
			TweenPosition.Begin(this.gameObject, 1f, offscreen);
			this.enabled = false;
		}
	}
}

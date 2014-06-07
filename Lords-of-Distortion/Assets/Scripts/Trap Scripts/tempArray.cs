using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tempArray : MonoBehaviour {

    //Make array~dictionary containing powers
    //Power Type and Sprite

    public Dictionary<PowerType, Sprite> icons;
    public Sprite glue;
    public Sprite smoke;
    public Sprite ink;
    public Sprite wind;
    private UISprite test;
    private UILabel testLabel;

    // Use this for initialization
	void Start () 
    {
        icons = new Dictionary<PowerType, Sprite>();
        icons.Add(PowerType.STICKY, glue);
       // icons.Add(PowerType.SMOKE, smoke);
        icons.Add(PowerType.FIREBALL, ink);
        icons.Add(PowerType.GRAVITY, wind);

        //We wont be able to use this when we add in multiple icons because they'll be labeled differently,
        //but its a start for now.
        
        //Something like this will change the background image of the sprite
        test = (UISprite) GameObject.Find("PowerSlot").GetComponent("UISprite");
        test.spriteName = ink.name;
        //Something like this will change the label text.
        testLabel = (UILabel)GameObject.Find("TriggerKey").GetComponent("UILabel");
        testLabel.text = "Q";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

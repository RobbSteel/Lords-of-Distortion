using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IconCooldowns : MonoBehaviour {

    PlayerServerInfo infoscript;
    private float timer = 0;
    public UISprite hookCDBtnRed;

    void Start()
    {
		infoscript = PlayerServerInfo.Instance;
    }	
    
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetButtonDown("Fire2"))
        {
            GameObject character = infoscript.GetPlayerGameObject(Network.player);
            timer = character.GetComponent<Hook>().hooktimer;
        }
        else
            timer -= Time.deltaTime;
        // The number in timer / x is the hooktimer value in Hook.cs
        hookCDBtnRed.fillAmount = timer / 3;
     
	}
}

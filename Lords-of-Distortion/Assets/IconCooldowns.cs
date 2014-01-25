using UnityEngine;
using System.Collections;

public class IconCooldowns : MonoBehaviour {

    private float timer = 0;
    public UISprite hookCDBtnRed;
    private GameObject character;

    void Awake()
    {
        character = GameObject.FindGameObjectWithTag("Player");
    }
    
	// Update is called once per frame
	void Update () 
    {
        if (character == null)
        {
            Debug.Log("Looking for Player");
            character = GameObject.FindGameObjectWithTag("Player");
        }
        else
            Debug.Log("Found Player");

        if (Input.GetMouseButton(0))
        {
            Debug.Log("Left Mouse Clicked");
            if (character != null)
            {
                Debug.Log("Setting Hook Timer");
                timer = character.GetComponent<Hook>().hooktimer;
            }

            Debug.Log("HookTimer from CD: " + timer);
        }
        else
            timer -= Time.deltaTime;

        hookCDBtnRed.fillAmount = timer / 5; //((Time.time - timer) / 5);
	}
}

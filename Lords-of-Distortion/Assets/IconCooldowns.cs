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
            character = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (Input.GetMouseButton(0))
        {
            if (character != null)
            {
                timer = character.GetComponent<Hook>().hooktimer;
            }
        }
        else
            timer -= Time.deltaTime;

        hookCDBtnRed.fillAmount = timer / 5; //((Time.time - timer) / 5);
	}
}

using UnityEngine;
using System.Collections;

public class SpellManager : MonoBehaviour {

    private GameObject fireball;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Update for SpellManager");
		if (Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("Instantiating Fireball");
			fireball = (GameObject)Instantiate(Resources.Load("fireball"));	
			
		}
	}
}
using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnClick()
    {
		var information = GameObject.Find("PSInfo");
		Destroy(information);
		Application.LoadLevel("MainMenu");

    }
}

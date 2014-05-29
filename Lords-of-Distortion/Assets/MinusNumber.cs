using UnityEngine;
using System.Collections;

public class MinusNumber : MonoBehaviour {

    public UILabel enterRoundsLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnPress(bool isDown)
    {
        if (isDown)
            return;
        Debug.Log("MINUS PRESSED");
        int rounds = int.Parse(enterRoundsLabel.text);
        if (rounds >= 1)
        {
            rounds--;
        }
        enterRoundsLabel.text = rounds.ToString();
    }
}

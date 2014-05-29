using UnityEngine;
using System.Collections;

public class RoundLimit : MonoBehaviour {

    private UILabel totalRoundsLabel;
    private int totalRounds;

	// Use this for initialization
	void Start () {
        totalRoundsLabel = gameObject.GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
        if(totalRoundsLabel.text != null)
        { 
            totalRounds = int.Parse(totalRoundsLabel.text);
        }
        else if (totalRoundsLabel.text != "1" || totalRoundsLabel.text != "2" ||
            totalRoundsLabel.text != "3" || totalRoundsLabel.text != "4" || totalRoundsLabel.text != "5")
        {
            totalRoundsLabel.text = "4";
            totalRounds = 4;
        }

        if(totalRounds <= 0)
        {
            totalRounds = 1;
            totalRoundsLabel.text = "1";
        }
        if(totalRounds >= 6)
        {
            totalRounds = 5;
            totalRoundsLabel.text = "5";
        }
	}
}

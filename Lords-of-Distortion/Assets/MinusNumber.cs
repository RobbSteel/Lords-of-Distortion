using UnityEngine;
using System.Collections;

public class MinusNumber : MonoBehaviour {

    public UILabel enterRoundsLabel;

    void OnPress(bool isDown)
    {
        if (isDown)
            return;

        int rounds = int.Parse(enterRoundsLabel.text);
        
        if (rounds > 1)
        {
            rounds--;
        }
        else if(rounds == 1)
        {
            rounds = 5;
        }
        enterRoundsLabel.text = rounds.ToString();
    }
}

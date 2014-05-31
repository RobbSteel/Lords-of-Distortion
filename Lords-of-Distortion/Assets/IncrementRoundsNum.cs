using UnityEngine;
using System.Collections;

public class IncrementRoundsNum : MonoBehaviour {

    public UILabel enterRoundsLabel;

    void OnPress(bool isDown)
    {
        if (isDown)
            return;
        
        int rounds = int.Parse(enterRoundsLabel.text);
        
        if (rounds < 5)
        {
            rounds++;
        }
        else if(rounds == 5)
        {
            rounds = 1;
        }
        enterRoundsLabel.text = rounds.ToString();
    }
}

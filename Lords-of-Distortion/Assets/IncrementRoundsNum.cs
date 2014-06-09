using UnityEngine;
using System.Collections;

public class IncrementRoundsNum : MonoBehaviour {

    public UILabel enterRoundsLabel;
	public AudioClip clicked;

    void OnPress(bool isDown)
    {
        if (isDown)
            return;
		if (!isDown) audio.PlayOneShot(clicked);

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

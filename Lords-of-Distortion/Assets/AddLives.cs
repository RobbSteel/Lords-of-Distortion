﻿using UnityEngine;
using System.Collections;

public class AddLives : MonoBehaviour {

    public UILabel enterLivesLabel;
	public AudioClip clicked;

    void OnPress(bool isDown)
    {
        if (isDown)
            return;
		if (!isDown) audio.PlayOneShot(clicked);
		
        int lives = int.Parse(enterLivesLabel.text);
        
        if (lives < 99)
        {
            lives++;
        }
        else if(lives == 99)
        {
            lives = 1;
        }

        enterLivesLabel.text = lives.ToString();
    }
}

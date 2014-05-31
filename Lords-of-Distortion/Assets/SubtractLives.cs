﻿using UnityEngine;
using System.Collections;

public class SubtractLives : MonoBehaviour {

    public UILabel enterLivesLabel;

    void OnPress(bool isDown)
    {
        if (isDown)
            return;

        int lives = int.Parse(enterLivesLabel.text);
        
        if (lives > 1)
        {
            lives--;
        }
        else if(lives == 1)
        {
            lives = 99;
        }
        
        enterLivesLabel.text = lives.ToString();
    }
}

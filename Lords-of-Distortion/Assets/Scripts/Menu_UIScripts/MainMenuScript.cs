using UnityEngine;

/// <summary>
/// Title screen script
/// </summary>
public class MainMenuScript : MonoBehaviour
{
    void OnGUI()
    {
        const int buttonWidth = 84;
        const int buttonHeight = 60;

        // Draw a button to start the game
        if (
          GUI.Button(
            // Center in X, 2/3 of the height in Y
            new Rect(
              Screen.width / 2 - (buttonWidth / 2),
              (2 * Screen.height / 3) - (buttonHeight / 2),
              buttonWidth,
              buttonHeight
            ),
            "Start!"
          )
        )
        {
            // On Click, load the first level.
            // "Stage1" is the name of the first scene we created.
            Application.LoadLevel("prototype");
        }

        // Draw a button to load options
        if (
          GUI.Button(
            // Center in X, 5/6 of the height in Y
            new Rect(
              Screen.width / 2 - (buttonWidth / 2),
              (5 * Screen.height / 6) - (buttonHeight / 2),
              buttonWidth,
              buttonHeight
            ),
            "Options"
          )
        )
        {
            // On Click, load the first level.
            // "Stage1" is the name of the first scene we created.
            Application.LoadLevel("Options");
        }
    }
}
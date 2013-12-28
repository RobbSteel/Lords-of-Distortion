using UnityEngine;
using System.Collections;

public class OptionsScript : MonoBehaviour 
{

    public float allVolumeSlider = 100.0f;
    public float bgmSlider = 100.0f;
    public float sfxSlider = 100.0f;
    const int buttonWidth = 84;
    const int buttonHeight = 60;

    void OnGUI()
    {

        // Optiosn Box. Contains all other boxes
        GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "Options");

        // All Volume Box
        GUI.Box(new Rect(50, (50 + Screen.height / 100), Screen.width - 100, 80), "All Volume");

        // All Volume Slider
        allVolumeSlider = GUI.HorizontalSlider(new Rect(75, 100 + Screen.height / 100, Screen.width - 160, 30), allVolumeSlider, 0.0f, 100.0f);

        // BGM Box
        GUI.Box(new Rect(50, 150 + Screen.height / 100, Screen.width - 100, 80), "BGM");

        // BGM Slider
        bgmSlider = GUI.HorizontalSlider(new Rect(75, 200 + Screen.height / 100, Screen.width - 160, 30), bgmSlider, 0.0f, 100.0f);

        // SFX Box
        GUI.Box(new Rect(50, 250 + Screen.height / 100, Screen.width - 100, 80), "SFX");

        // SFX Slider
        sfxSlider = GUI.HorizontalSlider(new Rect(75, 300 + Screen.height / 100, Screen.width - 160, 30), sfxSlider, 0.0f, 100.0f);

        if (
            GUI.Button(
            // Center in X, 2/3 of the height in Y
                new Rect(
                    Screen.width / 2 - (buttonWidth / 2),
                    (5 * Screen.height / 6) - (buttonHeight / 2),
                    buttonWidth,
                    buttonHeight
                ),
                "Back"
            )
        )
        {
            // On Click, load the first level.
            // "Stage1" is the name of the first scene we created.
            Application.LoadLevel("MainMenu");
        }
    }

}

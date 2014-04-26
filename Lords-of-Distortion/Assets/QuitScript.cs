using UnityEngine;
using System.Collections;

public class QuitScript : MonoBehaviour {
    
    void OnPress()
    {
        Application.Quit();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

}

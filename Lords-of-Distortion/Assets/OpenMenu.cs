using UnityEngine;
using System.Collections;

public class OpenMenu : MonoBehaviour {

    private bool pauseActive = false;
    public GameObject menuHolder;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseActive)
            {
                menuHolder.SetActive(true);
                pauseActive = true;
            }
            else
            { 
                menuHolder.SetActive(false);
                pauseActive = false;
            }
        }
    }

}

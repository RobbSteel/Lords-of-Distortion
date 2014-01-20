using UnityEngine;
using System.Collections;

public class countdown : MonoBehaviour {

    public UILabel myLabel;

    public float myTimer = 120.0f;

	// Update is called once per frame
    
    void Update()
    {
        if (myTimer > 0)
        {
            myTimer -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(myTimer / 60F);
            int seconds = Mathf.FloorToInt(myTimer - minutes * 60);

            string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            myLabel.text = niceTime;
            //Application.LoadLevel(1);
        }

    }
}

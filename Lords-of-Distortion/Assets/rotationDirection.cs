using UnityEngine;
using System.Collections;

public class rotationDirection : MonoBehaviour {

    private bool rotationEnabled = false;

    private Camera cam;
    private Vector3 firstClick;
    private Vector3 aimTowards;
    
	// Use this for initialization
	void Start () 
    {
        cam = GameObject.Find("MouseCamera").camera;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Deals with rotation for line
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 lookPos = cam.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
 
        //Creates the dotted line at first click. This would happen when you
        // already have the fireball/ink following your mouse and click to choose its placement.
        if(Input.GetMouseButtonUp(0) && !rotationEnabled)
        {
            firstClick = Input.mousePosition;
            firstClick.z = 10.0f;
            GameObject dottedLine = (GameObject)Instantiate(Resources.Load("DottedLine"));
            dottedLine.transform.position = cam.ScreenToWorldPoint(mousePos);
            rotationEnabled = true;
        }
 
        //Destroys the dotted line at second click. This would happen when you set the rotation. 
        else if(Input.GetMouseButtonUp(0) && rotationEnabled)
        {
            Destroy(GameObject.Find("DottedLine(Clone)"));
            rotationEnabled = false;
        }
        
	}
}

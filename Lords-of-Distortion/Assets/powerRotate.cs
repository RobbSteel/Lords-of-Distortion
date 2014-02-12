using UnityEngine;
using System.Collections;

public class powerRotate : MonoBehaviour {

    private Camera cam;
    
    // Use this for initialization
	void Awake () 
    {
        cam = Camera.main;//s GameObject.Find("MouseCamera").camera;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 lookPos = cam.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}

﻿using UnityEngine;
using System.Collections;

public class powerRotate : MonoBehaviour {

    private Camera cam;
	float originalAngle;
    // Use this for initialization
	void Awake () 
    {
        cam = Camera.main;//s GameObject.Find("MouseCamera").camera;
		originalAngle = transform.rotation.eulerAngles.z;
	}
	
	// Update is called once per frame
	void Update () 
    {
		Vector3 mousePos = new Vector3(GameInput.instance.MousePosition.x, GameInput.instance.MousePosition.y, 10f);
        Vector3 lookPos = cam.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
		float angle = originalAngle +  Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}

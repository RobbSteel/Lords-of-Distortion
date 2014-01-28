﻿using UnityEngine;
using System.Collections;

public class rotateToMouse : MonoBehaviour
{
	bool aiming;
	public float speed = 0;
	

	void Awake(){
		aiming = true;
		
	}
	
	
	// Use this for initialization
	void Start ()
	{
		
		
	}
	
	
	
	// Update is called once per frame
	void Update (){

			float dist = Vector3.Distance(new Vector3(0,0,0), transform.position);
			if (dist >= 7.0f) {
						//Destroy (this.gameObject);
				
			transform.Translate (speed, 0, 0);
			
			if(aiming) {
				
				var mousePos = Input.mousePosition;
				mousePos.z = 10.0f; //The distance from the camera to the player object
				Vector3 lookPos = Camera.main.ScreenToWorldPoint (mousePos);
				lookPos = lookPos - transform.position;
				float angle = Mathf.Atan2 (lookPos.y, lookPos.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
				
				//transform.LookAt(Camera.main.ScreenToWorldPoint (mousePos));
				
			}
			
			if (Input.GetMouseButton(0) == true) {
				aiming = false;
				speed = .1f;
				
			}

		}
	}
}
	

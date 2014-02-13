using UnityEngine;
using System.Collections;


public class ShipSway : MonoBehaviour {
public float minAngle = 100.0f;
public float maxAngle = 10.0f;
public float speed = 1000.0f;

	// Use this for initialization
	void Start () {	
	
	}

	// Update is called once per frame
	void Update () {
		//Math which rotates the ship between max and min angles
		float angle = Mathf.LerpAngle(minAngle, maxAngle, Mathf.PingPong(Time.time / speed, 1.0f));
		transform.eulerAngles = new Vector3(20, angle, 10);
	}
}

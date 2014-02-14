using UnityEngine;
using System.Collections;


public class ShipSway : MonoBehaviour {
public float minAngle = 100.0f;
public float maxAngle = 10.0f;
public float speed = 1000.0f;

	public float frequencyMin = 1.0f;
	public float frequencyMax = 2.0f;
	public float magnitude = 0.005f;
	private float randomInterval;

	// Use this for initialization
	void Start () {	
		randomInterval = Random.Range (frequencyMin, frequencyMax);
	}

	// Update is called once per frame
	void Update () {
		//Math which rotates the ship between max and min angles
		Vector2 temp = transform.position;
		temp.y += (Mathf.Sin(Time.time * randomInterval) * magnitude);
		transform.position = temp;

		Vector2 temp2 = transform.eulerAngles;
		temp2.x += (Mathf.Sin(Time.time * randomInterval) * 0.01f);
		transform.eulerAngles = temp2;



		//float angle = Mathf.LerpAngle(minAngle, maxAngle, Mathf.PingPong(Time.time / speed, 1.0f));
		//transform.eulerAngles = new Vector3(20, angle, 10);
	}
}

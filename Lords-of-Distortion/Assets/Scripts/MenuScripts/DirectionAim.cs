using UnityEngine;
using System.Collections;

public class DirectionAim : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mousePos = new Vector3(GameInput.instance.MousePosition.x,  GameInput.instance.MousePosition.y, 10f);
		Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
		lookPos = lookPos - transform.position;
		float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}

using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {
    private Camera camera;
   

	// Use this for initialization
	void Start ()
    {
	    camera = Camera.main;
	}

	// Update is called once per frame
	void Update ()
    {
		transform.position = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
		Vector3 temp = new Vector3(0, 0, 10.0f);
		transform.position += temp;
    }

}

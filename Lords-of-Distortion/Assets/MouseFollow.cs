using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {
    public Camera camera;
   

	// Use this for initialization
	void Start ()
    {
	    camera = Camera.main;
	}

	//TODO: Instead of re-centering object, retain offset from mouse.
	void Update ()
    {
		Vector3 worldPosition  = camera.ScreenToWorldPoint(new Vector3(GameInput.instance.MousePosition.x, GameInput.instance.MousePosition.y, 0.0f));
		worldPosition.z = 10f;
		transform.position = worldPosition;

    }

}

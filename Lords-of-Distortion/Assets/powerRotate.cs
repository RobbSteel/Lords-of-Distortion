using UnityEngine;
using System.Collections;
using InControl;
public class powerRotate : MonoBehaviour {

    private Camera cam;
	float originalAngle;
	public float angle;
    // Use this for initialization
	void Awake () 
    {
        cam = Camera.main;
		originalAngle = transform.rotation.eulerAngles.z;
	}

	void OnEnable(){
		GameInput.instance.HideCustomCursor();
	}

	void OnDisable(){
		GameInput.instance.UseCustomCursor();
	}
	
	// Update is called once per frame
	Vector3 oldMouse;
	void Update () 
    {
		if(GameInput.instance.usingGamePad)
		{
			InputDevice device = InputManager.ActiveDevice;
			Vector2 inputVector = device.RightStick.Vector;
			if(inputVector.sqrMagnitude > .1f)
				angle = originalAngle + Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
		}
		else if(oldMouse != Input.mousePosition)
		{
			Vector3 mousePos = new Vector3(GameInput.instance.MousePosition.x, GameInput.instance.MousePosition.y, 10f);
			Vector3 lookPos = cam.ScreenToWorldPoint(mousePos);
			lookPos = lookPos - transform.position;
			angle = originalAngle +  Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
		}

        gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		oldMouse = Input.mousePosition;
	}
}

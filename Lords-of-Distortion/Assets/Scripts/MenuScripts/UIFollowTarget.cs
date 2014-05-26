using UnityEngine;
using System.Collections;

public class UIFollowTarget : MonoBehaviour {

	public Transform Target;
	public Vector2 offsetScale;

	void LateUpdate(){

		if(Target == null)
		{
			this.enabled = false;
			return;
		}

		Vector3 screenPos = Camera.main.WorldToScreenPoint(Target.position);
		Vector3 viewPos = Camera.main.WorldToViewportPoint(Target.position);
		Vector3 uiScreenPos = UICamera.currentCamera.ViewportToScreenPoint(viewPos);
		// Convert the position from world to screen so we know where to position it
		Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
		
		// Add the offset
		screenPos.y += Screen.height * offsetScale.y;
		screenPos.x = uiScreenPos.x * 720f / Screen.height;
		screenPos.x -= Screen.width * 720f / Screen.height /2f;
		
		// Move the element to the right position
		transform.localPosition = screenPos;
	}
}

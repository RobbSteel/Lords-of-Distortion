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
			
		// Convert the position from world to screen so we know where to position it
		Vector3 screenPos = Camera.main.WorldToScreenPoint(Target.position);
		
		// need to remove half the width and half the height since our NGUI 0, 0 is in the middle of the screen
		float screenHeight = Screen.height;
		float screenWidth = Screen.width;
		screenPos.x -= (screenWidth / 2.0f);
		screenPos.y -= (screenHeight / 2.0f);
		
		// Add the offset
		screenPos.x += offsetScale.x;
		screenPos.y += screenHeight * offsetScale.y;
		
		// Move the element to the right position
		transform.localPosition = screenPos;
	}
}

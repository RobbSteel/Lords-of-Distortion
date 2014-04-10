using UnityEngine;
using System.Collections;

public class UIFollowTarget : MonoBehaviour {

	public Transform Target;
	public Vector2 offset;

	void Update(){
		if(Target == null)
			this.enabled = false;
		// Convert the position from world to screen so we know where to poistion it
		Vector3 screenPos = Camera.main.WorldToScreenPoint(Target.position);
		
		// need to remove half the width and half the height since our NGUI 0, 0 is in the middle of the screen
		float screenHeight = Screen.height;
		float screenWidth = Screen.width;
		screenPos.x -= (screenWidth / 2.0f);
		screenPos.y -= (screenHeight / 2.0f);
		
		// Add the offset
		screenPos.x += offset.x;
		screenPos.y += offset.y;
		
		// Move the element to the right position
		transform.localPosition = screenPos;
	}
}

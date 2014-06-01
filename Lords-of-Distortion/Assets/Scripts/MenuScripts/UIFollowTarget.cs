using UnityEngine;
using System.Collections;

public class UIFollowTarget : MonoBehaviour {

	public Transform Target;
	public Vector2 offsetScale;

	const float fixedHeight = 720f;

	Vector3 screenPos;
	void LateUpdate(){
		float UIToScreenRatio = fixedHeight / Screen.height;
		if(Target == null)
		{
			this.enabled = false;
			return;
		}

		Vector3 viewPos = Camera.main.WorldToViewportPoint(Target.position);
		Vector3 uiScreenPos = UICamera.currentCamera.ViewportToScreenPoint(viewPos);

		screenPos.y = uiScreenPos.y * UIToScreenRatio;
		//move to center.
		screenPos.y -= fixedHeight / 2f;
		//add offset.
		screenPos.y += fixedHeight * offsetScale.y;

		screenPos.x = uiScreenPos.x * UIToScreenRatio;
		//move to center
		screenPos.x -= Screen.width * UIToScreenRatio /2f;
		
		// Move the element to the right position
		transform.localPosition = screenPos;
	}
}

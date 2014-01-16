using UnityEngine;
using System.Collections;

public class SmoothFollow2D : MonoBehaviour {

	public Transform target;
	public float smoothTime;
	private Transform thisTransform;
	private Vector2 velocity;

	// Use this for initialization
	void Start () {
		thisTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if( target != null ){
			Vector3 newPos = new Vector3( Mathf.SmoothDamp( thisTransform.position.x, target.position.x, ref velocity.x, smoothTime),
			                            Mathf.SmoothDamp( thisTransform.position.y, target.position.y, ref velocity.y, smoothTime),
			                             thisTransform.position.z);

			thisTransform.position = newPos;
		}
	}
}



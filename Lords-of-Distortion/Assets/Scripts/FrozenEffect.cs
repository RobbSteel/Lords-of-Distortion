using UnityEngine;
using System.Collections;

public class FrozenEffect : MonoBehaviour {

	public Controller2D checkStun;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		DestroyEffect();
	}

	void DestroyEffect(){
		if (checkStun != null) {
			transform.position = checkStun.transform.position;
			if (!checkStun.stunned) {
					Destroy (this.gameObject);
			}
		}
	}


}

using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnCollisionStay2D(Collision2D col ) {
		if (col.gameObject.tag == "Player")
			Destroy( col.gameObject );
		Debug.Log( col.gameObject.name );
	}
}
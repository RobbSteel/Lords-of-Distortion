using UnityEngine;
using System.Collections;

public class DestroyObjective : MonoBehaviour {

	public GameObject reference;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DestroyOnNull ();
	}


	void DestroyOnNull(){
		if (reference == null) {
			Destroy( this.gameObject );
		}

	}
}

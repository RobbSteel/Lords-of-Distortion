using UnityEngine;
using System.Collections;

public class TrapFountain : MonoBehaviour {

	bool used = false;

	// Use this for initialization
	void Start () {
		Destroy(gameObject, 6f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player"){
			if(!used){
				used = true;
				audio.Play();
			}
		}
	}
}

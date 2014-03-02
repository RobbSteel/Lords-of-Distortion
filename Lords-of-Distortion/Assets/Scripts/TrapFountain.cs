using UnityEngine;
using System.Collections;

public class TrapFountain : MonoBehaviour {

	bool used = false;
	public PlacementUI placementUI;

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
				placementUI.Resupply();
				used = true;
				audio.Play();
			}
		}
	}
}

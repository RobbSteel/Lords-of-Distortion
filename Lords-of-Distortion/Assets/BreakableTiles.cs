using UnityEngine;
using System.Collections;

public class BreakableTiles : MonoBehaviour {

	private Melee meleeCheck;
	private MeleewithoutNetwork isMeleeing;
	// Use this for initialization
	void Awake () {
		meleeCheck = GetComponent<Melee>();
		isMeleeing = GetComponent<MeleewithoutNetwork> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter2D(Collider2D col){
		if(col.gameObject.tag == "Melee"){ 
			Debug.Log ("Got here");
						
		}
	}
}
	

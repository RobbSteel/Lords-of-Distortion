using UnityEngine;
using System.Collections;

public class PillarCrush : MonoBehaviour {


	public GameObject endPosition;
	public GameObject pillarPosition;
	private string PlayerTag = "Player";
	private BoxCollider2D pillarKillCollider;
	public float playerSize;

	private Vector3 lastPosition;
	// Use this for initialization
	void Start () {
		pillarKillCollider = this.GetComponent<BoxCollider2D> ();
		pillarKillCollider.enabled = false;
		lastPosition = pillarPosition.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		calculateCrush();
	}

	void calculateCrush(){
		if( Vector3.Distance( endPosition.transform.position, pillarPosition.transform.position ) <= playerSize 
		   && checkDirection() ){
			pillarKillCollider.enabled = true;
		}else{
			pillarKillCollider.enabled = false;
		}

	}

	bool checkDirection(){
		//going down kill player
		//and set current position to lastposition
		if (lastPosition.y >= pillarPosition.transform.position.y) {
			lastPosition = pillarPosition.transform.position;
			return true;
		}else{
			lastPosition = pillarPosition.transform.position;
			return false;
		}
	}

	void OnCollisionEnter2D(Collision2D other){
		Debug.Log ( other.transform.name + " Got Crushed"); 
		if (other.transform.CompareTag(PlayerTag)) {
			other.transform.GetComponent<Controller2D>().Die(DeathType.CRUSH);
		}
	}

}

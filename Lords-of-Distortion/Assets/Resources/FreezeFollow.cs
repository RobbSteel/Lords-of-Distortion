using UnityEngine;
using System.Collections;

public class FreezeFollow : MonoBehaviour {

	public GameObject followplayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(followplayer != null){
		transform.position = Vector2.MoveTowards(transform.position, followplayer.transform.position, 10);

		}
	}
}

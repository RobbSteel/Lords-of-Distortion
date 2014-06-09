using UnityEngine;
using System.Collections;

public class CustomButton : MonoBehaviour {
	
	private GameObject currentTexture;
	public AudioClip buttonhover;
	
	// Use this for initialization
	void Start () {
		currentTexture = this.transform.GetChild (0).gameObject;
	}
	
	void OnHover( bool isOver ){
		if (isOver){
			audio.PlayOneShot(buttonhover, 0.25f);
		}
	}
}

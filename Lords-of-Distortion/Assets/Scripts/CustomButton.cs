using UnityEngine;
using System.Collections;

public class CustomButton : MonoBehaviour {

	//public Texture onClickTexture;
	//public Texture onHoverTexture;
	//public Texture defaultTexture;
	private GameObject currentTexture;
	public AudioClip buttonhover;
	//private bool pressed;

	void OnAwake(){
		//get child with texture

	}

	// Use this for initialization
	void Start () {
		currentTexture = this.transform.GetChild (0).gameObject;
		//defaultTexture = currentTexture.GetComponent<UITexture> ().mainTexture;
		//pressed = false;


		//if (onClickTexture == null)
			//onClickTexture = currentTexture.GetComponent<UITexture> ().mainTexture;

	}

	//conflicts with on hover giving it a problem using both
	/*void OnPress( bool isDown ){
		if (isDown) {
			pressed = true;
			currentTexture.GetComponent<UITexture> ().mainTexture = onHoverTexture;
		} 
		else
			pressed = false;

	}*/



	void OnHover( bool isOver ){
	
		if (isOver){
			audio.PlayOneShot(buttonhover, 0.35f);
		}

	}
}

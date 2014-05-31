using UnityEngine;
using System.Collections;



public class Transitioner : MonoBehaviour {
	public GameObject curlroot;
	public GameObject uiroot;
	public bool fadeaway;
	public GameObject remove;
	public bool fadein;
	public UIPanel fadepanel;
	PageCurl curler;

	// Use this for initialization
	void Start () {

		fadepanel = uiroot.GetComponent<UIPanel>();
		curler = curlroot.GetComponent<PageCurl>();
		fadein = true;
	}

	public void Flip(string loadlevel, bool flipright){

		curler.Flip(loadlevel, flipright);
		fadeaway = true;
		Destroy (remove);
	}

	void Update(){
		if(fadeaway){
			fadepanel.alpha -= 0.1f;
		} else if(fadein){
			fadepanel.alpha += 0.1f;
			if(fadepanel.alpha == 1){
				fadein = false;
			}
		}
		
	}

}

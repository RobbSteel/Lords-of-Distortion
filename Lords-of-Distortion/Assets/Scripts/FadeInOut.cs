using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

	public float fadeSpeed = 0.3f;
	public int drawDepth = -1000;
	public float maxFade = 0.7f;
	public float minFade = 0f;

	private Texture2D fadeOutTexture;
	private float alpha = 0;
	private float fadeDir = -1;

	void Awake(){
		fadeOutTexture = (Texture2D)Resources.Load ("black");
	}

	void OnGUI(){
		alpha += fadeDir * fadeSpeed * Time.deltaTime;	
		alpha = Mathf.Clamp(alpha , minFade , maxFade);	

		Vector4 newColor = Vector4.one * alpha;
		GUI.color = newColor;
		
		GUI.depth = drawDepth;
		Rect rec = new Rect(0, 0, Screen.width, Screen.height);
		GUI.DrawTexture ( rec , fadeOutTexture );

	}

	void fadeIn(){
		fadeDir = -1;
	}

	void fadeOut(){
		fadeDir = 1;
	}

	// Use this for initialization
	void Start () {
		alpha = 0;
	}

}

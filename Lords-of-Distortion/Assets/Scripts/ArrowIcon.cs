using UnityEngine;
using System.Collections;

public class ArrowIcon : MonoBehaviour {

	[Range(0, 1)]
	public float alpha;

	// Use this for initialization
	void Start () {
		SpriteRenderer iconRender = this.GetComponent<SpriteRenderer> ();
		Vector4 iconNewColor = iconRender.color;
		iconNewColor.w = alpha;
		iconRender.color = iconNewColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}

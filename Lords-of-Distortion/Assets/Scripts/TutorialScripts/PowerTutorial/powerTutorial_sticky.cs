using UnityEngine;
using System.Collections;

public class powerTutorial_sticky : MonoBehaviour {

	public Animator anim;
	//public GameObject powerDemo;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnHover(bool isOver){
		if (isOver) {
			anim.SetBool("sticky_hover", isOver);
		}
		if (!isOver) {
			anim.SetBool("sticky_hover", isOver);
			
		}
	}
}

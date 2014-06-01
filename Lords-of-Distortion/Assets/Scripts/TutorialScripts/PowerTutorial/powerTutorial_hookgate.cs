﻿using UnityEngine;
using System.Collections;

public class powerTutorial_hookgate : MonoBehaviour {

	public Animator anim;
	public AudioClip buttonhover;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnHover(bool isOver){
		if (isOver) {
			audio.PlayOneShot(buttonhover, 0.35f);
			anim.SetBool("hookgate_hover", isOver);
		}
		if (!isOver) {
			anim.SetBool("hookgate_hover", isOver);
			
		}
	}
}

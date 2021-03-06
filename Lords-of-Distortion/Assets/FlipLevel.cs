﻿using UnityEngine;
using System.Collections;

public class FlipLevel : MonoBehaviour {
	
	[SerializeField]
	private string LevelName = "MainMenu";
	public bool flipleft = false;
	public GameObject transitioner;
	public AudioClip buttonclick;
	public AudioClip buttonhover;

	Transitioner fader;
	
	void Awake(){
		
		fader = transitioner.GetComponent<Transitioner>();
	}
	
	void OnPress(bool isDown)
	{
        if (isDown)
            return;
		audio.PlayOneShot(buttonclick);
		fader.Flip(LevelName, flipleft);
	}
	void OnHover(bool isOver){
		if (isOver)
			audio.PlayOneShot (buttonhover, 0.2f);
	}
}
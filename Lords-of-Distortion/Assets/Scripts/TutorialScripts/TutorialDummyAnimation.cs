using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimationToPlay{ Run = 0 , Melee, Jump }


public class TutorialDummyAnimation : MonoBehaviour {
	private Animator dummyAnimation;
	public AnimationToPlay currentAnimation;



	// Use this for initialization
	void Start () {
		dummyAnimation = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		SetupAnimation ();
	}


	//wont work unless the animator for the controller substates are set correctly to transition correctly
	//-----------------may be maded easier by playing around with the anystate substate but it loops incorrectly
	void SetupAnimation(){
		switch (currentAnimation) {
			case AnimationToPlay.Jump:
				dummyAnimation.SetInteger( "AnimationToPlay" , 2 );
				break;
			case AnimationToPlay.Run:
				dummyAnimation.SetInteger( "AnimationToPlay" , 0 );
				break;
			case AnimationToPlay.Melee:
				dummyAnimation.SetInteger( "AnimationToPlay" , 1 );
				break;
			default:
				break;
		}
	}
}

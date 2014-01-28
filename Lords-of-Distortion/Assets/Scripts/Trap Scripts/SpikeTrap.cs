using UnityEngine;
using System.Collections;

public class SpikeTrap : Power {

	public bool movingSpike;
	public float durationOfTween;
	public float delay;

	private TweenPosition spikeTween;
	private Vector3	setCorrectPosition;
	private GameObject childTweenPoint;


	void Awake(){
		childTweenPoint = transform.FindChild("TweenPosition").gameObject;
		setCorrectPosition = transform.position;
		spikeTween = this.GetComponent<TweenPosition>();
		spikeTween.duration = durationOfTween;
		spikeTween.delay = delay;
		spikeTween.from = setCorrectPosition;
		SetUpTweenPosition();

	}


	private void SetUpTweenPosition(){
		spikeTween.from = setCorrectPosition;
		spikeTween.to = childTweenPoint.transform.position;
	}

	//Toggles spikes back and forth for its animation duration 
	//Then 
	private void MovingSpike(){
				if( spikeTween.enabled == false && movingSpike ){
					spikeTween.enabled = true;
					spikeTween.Toggle();
				}
	}

	void Update(){
		MovingSpike();
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){
		controller.Die();
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){

	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){

	}
}
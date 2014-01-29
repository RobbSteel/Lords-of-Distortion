using UnityEngine;
using System.Collections;

public class SpikeTrap : Power {

	public bool movingSpike;
	public float durationOfTween;
	public float delay;

	private TweenPosition spikeTween;
	private Vector3	startPosition;
	private GameObject childTweenPoint;



	void Awake(){
		//grabs child refences that is the refernce for the To Position
		childTweenPoint = transform.FindChild("TweenPosition").gameObject;
		startPosition = transform.position;
		spikeTween = this.GetComponent<TweenPosition>();
		spikeTween.duration = durationOfTween;
		spikeTween.delay = delay;
		spikeTween.from = startPosition;
		SetUpTweenPosition();

	}

	//Sets up spikes tweening from its tween position to its starting position
	private void SetUpTweenPosition(){
		spikeTween.from = startPosition;
		spikeTween.to = childTweenPoint.transform.position;
	}

	//Toggles spikes back and forth for its animation duration 
	//Then 
	private void MovingSpike(){
				if( spikeTween.enabled == false && movingSpike ){
					spikeTween.enabled = true;
					spikeTween.Toggle();//reverse animation
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
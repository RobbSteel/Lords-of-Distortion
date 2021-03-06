﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NetworkController : MonoBehaviour {
	public bool DEBUG;

	public NetworkPlayer theOwner;
    private Controller2D controller2D;
    
	public bool isOwner = false;

	private float width;


	//A buffer of states. Not sure if a circular buffer is the best data structure at this point.
	private CircularBuffer<State> states;

	/* Synchronization Variables */                                                                                                                                                                                                                                      
	private float currentSmooth = 0f;
	private bool canInterpolate= false;
	private bool canExtrapolate = false;
	private float simTime;
	

	/*
	 * Holds various properties required to simulate the state of a player locally.
	 */
	struct State{
		public Vector3 position;
		public Vector3 velocity;
		public bool facingRight;
		public bool crouching;
		public double remoteTime; //the time of this state on the client
		public float localTime;  //the time we received a copy of this state.
		public State(Vector3 position, bool facingRight){
			this.position = position;
			this.velocity = Vector3.zero;
			this.facingRight = facingRight;
			this.remoteTime = double.NaN;
			this.localTime = float.NaN;
			this.crouching = false;
		}
	}

	void Awake() {
		controller2D = GetComponent<Controller2D>();
		states = new CircularBuffer<State>(20);
	}

	void Start () {
		BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
		width = collider.size.x;
	}

	float yVel = 0f;
	float prevYVel = 0f;
	float prevYPosition = 0f;
	void FixedUpdate(){
		if(isOwner)
			return;
		/*
		yVel = transform.position.y - prevYPosition;

		print (prevYVel);
		if(controller2D.grounded && prevYVel <= 0.00001f && yVel > 0.00001f)
			controller2D.anim.SetTrigger("Jump");

		prevYVel = yVel;
		prevYPosition = transform.position.y;
		*/
	
	}

	float interval = float.PositiveInfinity;
	

	public void SetOwner(NetworkPlayer owner){
		theOwner = owner;
		if(owner == Network.player){ 
			isOwner = true; //we can control the player locally
		}
		else {
			rigidbody2D.isKinematic = true;
			//put on other player layer
			gameObject.layer = 13;
		}
	}

	Vector3 estimatedVelocity;
	Vector3 latestPosition;

	State latestState;
	//The backup delay between players, in case packets drop.
	public static double interpolationDelay = 0.10;

	public double ConnectionPing{
		private set{
			ping = value;
		}

		get{
			return ping;
		}
	}

	[SerializeField]
	private double ping = 0.0;
	double LerpDouble(double from, double to, double t){
		if(t > 1.0)
			t = 1.0;
		return (1.0-t) * from + t * to;
	}

	double pingLerpRate = 1.0;
	float maxExtrapolation = .25f;
	float interpolations = 0f;
	float updates = 0f;

	public float interpolationPercentage;
	
	bool jumped = false;
    bool crouched = false;
	void Update () {

		if (isOwner)
			return;
		updates++;
		//Take into consideration ping between players when setting simulation time.
		//We want it to be smoothed, so that fluctuating ping doesnt cause jittering
		smoothPing = LerpDouble(smoothPing, ping, (double)Time.deltaTime);
		//The time we would like to see the other player. We are seeing where they were in the past
		double simulationTime = Network.time - smoothPing - interpolationDelay;

		//Check to see if we have something newer than our desired simulation time,
		//so we can interpolate. This also implies that there is a state which has a time further
		//in the past than our desired sim time, because we collected states until our desired state
		//came up.
		if(states.Count > 1 && states.ReadNewest().remoteTime > simulationTime){
			int i = states.Count - 1;
			foreach(State state in states.Reverse<State>()){

				//Look for state that is (interpolation delay + 1 packet delay) in the past
				//If we don't have something that old, use the oldest state possible.

				if(state.remoteTime <= simulationTime || i == 0){
					//get a state 1 newer than what we have
					State newerState = states.GetByIndex(i + 1);
					State olderState = state;

					double interval = newerState.remoteTime - olderState.remoteTime;
					//Because packets dont match up perfectly with frames, interpolate a certain amount
					//of time past the last packet.
					double timePassed = simulationTime - olderState.remoteTime;

					transform.position = Vector3.Lerp(olderState.position, newerState.position,
					                                  (float)(timePassed/interval));
					//flip player if necessary
					if(newerState.facingRight != controller2D.facingRight)
						controller2D.Flip();


					//If player is in air, play jump animation, otherwise play ground animation.
					if (newerState.velocity.y > 1f)
					{
						if (!controller2D.grounded && !jumped && !GetComponent<Hook>().HookOut)
						{
							jumped = true;
							controller2D.anim.SetTrigger("Jump");
						}
					} 
					else if(controller2D.grounded)
					{ 
						jumped = false;
					}

                    if(newerState.crouching && !olderState.crouching && !controller2D.crouching)
                    {
                        controller2D.SetCrouchState(true);
                    }
                    else if(olderState.crouching && !newerState.crouching && controller2D.crouching)
                    {
                        controller2D.SetCrouchState(false);
                    }
                
					float unit = Mathf.Abs(newerState.position.x - olderState.position.x) > .01f ? 1.0f : 0.0f;
					controller2D.anim.SetFloat( "Speed", unit);
					interpolations++;
					return;
				}
				i--;
			}
		}

		else {
			State latest = states.ReadNewest();

			float extrapolationLength = (float)(simulationTime - latest.remoteTime);
			// Don't extrapolate for more than 250 ms
			if (extrapolationLength < maxExtrapolation)
			{
				transform.position = latest.position + latest.velocity * extrapolationLength;
				rigidbody2D.velocity = latest.velocity;
			}

			interpolationPercentage = interpolations/updates  * 100f;

			//print ("Interpolating " + (interpolations/updates  * 100f)+  "% of the time.");
		}
	}


	void OnDestroy(){
		//remove self from dictionary since gameobject will be invalid.
		if(!DEBUG)
			SessionManager.Instance.psInfo.playerObjects.Remove(theOwner);
	}
	double smoothPing;

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){

		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		bool syncFacingRight = false;
		bool syncCrouch = false;
		if (stream.isWriting)
		{
			//if we have control over this entity, send out our positions to everyone else.
			syncPosition = transform.position;
			syncVelocity = rigidbody2D.velocity;

			syncCrouch = controller2D.crouching;
			syncFacingRight = controller2D.facingRight;

			stream.Serialize(ref syncPosition.x);
			stream.Serialize(ref syncPosition.y);
			stream.Serialize(ref syncVelocity.x);
			stream.Serialize(ref syncVelocity.y);
			stream.Serialize(ref syncFacingRight);
			stream.Serialize(ref syncCrouch);
		}

		else {

			ConnectionPing = Network.time - info.timestamp;
			//reject out of order/duplicate packets

			if(states.Count >= 2){
				double newestTime = states.ReadNewest().remoteTime;
				if(info.timestamp >= newestTime + 1f/Network.sendRate * 2.0f){
					Debug.Log("Delay: " + (newestTime - info.timestamp) + " (s)");
				}
				else if(info.timestamp < newestTime) {
					Debug.Log("out of order packet");
					return;
				}
				else if(info.timestamp == newestTime){
					Debug.Log("duplicate packet");
					return;
				}
			}
			
	
			//Write syncrhronized values to a state.
			stream.Serialize(ref syncPosition.x);
			stream.Serialize(ref syncPosition.y);

			stream.Serialize(ref syncVelocity.x);
			stream.Serialize(ref syncVelocity.y);

			stream.Serialize(ref syncFacingRight);
			stream.Serialize(ref syncCrouch);


			State state = new State(syncPosition, syncFacingRight);
			state.velocity = syncVelocity;
			state.crouching = syncCrouch;
			state.remoteTime = info.timestamp;
			state.localTime = (float)Network.time;
			states.Add(state); //if we advanced buffer manually, then count < maxsize
			//print ("----- " + state.remoteTime);
			if(canInterpolate == false && states.Count >= 3){
				rigidbody2D.isKinematic = true;
				canInterpolate = true;
				//in case we were extrapolating
				if(canExtrapolate){
					//remove oldest state from the buffer since we dont need it
					canExtrapolate = false;
					currentSmooth = (float)(Network.time - latestState.remoteTime);
					states.DiscardOldest();
				}
			}
		}
	}
}

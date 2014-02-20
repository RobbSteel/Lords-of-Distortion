using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {
	public bool DEBUG;
	public SessionManager instanceManager;

	public NetworkPlayer theOwner;
	private Controller2D controller2D;

	public bool isOwner = false;

	private float width;

	public GameObject DeathSpirit;

	//A buffer of states. Not sure if a circular buffer is the best data structure at this point.
	private CircularBuffer<State> states;

	/* Synchronization Variables */                                                                                                                                                                                                                                      
	private float currentSmooth = 0f;
	private bool canInterpolate= false;
	private float simTime;


	/*
	 * Holds various properties required to simulate the state of a player locally.
	 */
	struct State{
		public Vector3 position;
		public bool facingRight;
		public bool inAir;
		public float remoteTime; //the time of this state on the client
		public float localTime;  //the time we received a copy of this state.
		public State(Vector3 position, bool facingRight){
			this.position = position;
			this.facingRight = facingRight;
			this.remoteTime = float.NaN;
			this.localTime = float.NaN;
			this.inAir = false;
		}
	}
	

	//just learned about this, might be useful later on
	void OnNetworkInstantiate(NetworkMessageInfo info){

	}

	//Sets the network ID to this instantiation of the player.
	[RPC]
	void SetPlayerID(NetworkPlayer player)
	{
		Debug.Log ("received owner id");
		theOwner = player;
		if(player == Network.player){ 
			isOwner = true; //we can control the player locally
		}
		else{
			rigidbody2D.isKinematic = true;
			states = new CircularBuffer<State>(4);
		}

		//we should have created a local playeroptions by now
		if(!DEBUG){
			instanceManager =  GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
			PlayerOptions playerOptions = instanceManager.gameInfo.GetPlayerOptions(theOwner);
			//Debug.Log("Player " + theOwner + " number " + playerOptions.PlayerNumber);
			if(instanceManager.gameInfo.GetPlayerGameObject(theOwner) == null)
				instanceManager.gameInfo.AddPlayerGameObject(theOwner, gameObject);
			SpriteRenderer myRenderer = gameObject.GetComponent<SpriteRenderer>();
			switch(playerOptions.style){

			case PlayerOptions.CharacterStyle.BLUE:
				myRenderer.color = Color.blue;
				break;
				
			case PlayerOptions.CharacterStyle.RED:
				myRenderer.color = Color.red;
				break;

			case PlayerOptions.CharacterStyle.GREEN:
				myRenderer.color = Color.green;
				break;
			}
		}
	}

	void Awake() {
		controller2D = GetComponent<Controller2D>();
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


	void Update () {
		if (isOwner)
			return;

		if(canInterpolate){
			currentSmooth += Time.deltaTime;
			//if we go past these two states, move to next one.
			if(currentSmooth >= interval){
				if(states.Count > 2){
					states.DiscardOldest();
					//if we were perfectly in sync, we could reset to 0, but instead set it to the amount we overshot
					currentSmooth = currentSmooth - interval; 
				}
				else {
					Debug.Log("Missed too many packets. We need 2 states to interpolate between.");
					currentSmooth = 0f;
					interval = float.PositiveInfinity;
					canInterpolate = false;
					//don't interpolate but let gravity do its job so that players dont freeze in air
					rigidbody2D.isKinematic = false;
				}
			}
			//read the two oldest states.
			State oldState = states.ReadOldest(); 
			State newState = states.GetByIndex(1);
			
			//tells us how long to interpolate between these two states.
			interval = newState.remoteTime - oldState.remoteTime;
			
			if(newState.facingRight != controller2D.facingRight)
				controller2D.Flip();
			//If player is in air, play jump animation, otherwise play ground animation.
			if(newState.inAir)
				controller2D.anim.SetTrigger("Jump");

			Vector3 positionDifference = newState.position - oldState.position;
			float distanceApart = positionDifference.magnitude;
			if(distanceApart > width * 20.0f){
				//snap to position if difference is too great.
				transform.position = newState.position;
				oldState.position = newState.position;
			}
			else
				transform.position = Vector3.Lerp(oldState.position, newState.position,
				                                  currentSmooth/(interval));
			//The local simulation time of this thing.
			simTime = oldState.remoteTime +  currentSmooth;

			//simulate animations.
			float unit = Mathf.Abs(newState.position.x - oldState.position.x) > .01f ? 1.0f : 0.0f;
			controller2D.anim.SetFloat( "Speed", unit);
		}
	}

	public void DestroyPlayer(){
		if (isOwner)
			Network.Destroy (gameObject);
			Network.Instantiate(DeathSpirit, transform.position, transform.rotation, 0);
	}

	void OnDestroy(){
		//remove self from dictionary since gameobject will be invalid.
		if(!DEBUG)
			instanceManager.gameInfo.playerObjects.Remove(theOwner);
	}
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){

		Vector3 syncPosition = Vector3.zero;
		bool syncFacing = false;
		bool syncInAir = false;
		if (stream.isWriting)
		{
			//if we have control over this entity, send out our positions to everyone else.
			syncPosition = transform.position;
			syncFacing = controller2D.facingRight;
			syncInAir = controller2D.inAir;

			stream.Serialize(ref syncPosition.x);
			stream.Serialize(ref syncPosition.y);
			stream.Serialize(ref syncFacing);
			stream.Serialize(ref syncInAir);
		}

		else {
			//reject out of order/duplicate packets
			if(states != null && states.Count >= 2){
				double newestTime = states.ReadNewest().remoteTime;
				if(info.timestamp >= newestTime + 1f/Network.sendRate * 2.0f){
					Debug.Log("lost previous packet");
					Debug.Log("local" + newestTime + "server" + info.timestamp);
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
			float xPosition = 0f;
			float yPosition = 0f;
			stream.Serialize(ref xPosition);
			stream.Serialize(ref yPosition);
			syncPosition = new Vector3(xPosition, yPosition, 0f);
			stream.Serialize(ref syncFacing);
			stream.Serialize(ref syncInAir);
			State state = new State(syncPosition, syncFacing);
			state.inAir = syncInAir;
			state.remoteTime = (float)info.timestamp;
			state.localTime = (float)Network.time;
			states.Add(state); //if we advanced buffer manually, then count < maxsize

			if(canInterpolate == false && states != null && states.Count >= 3){
				rigidbody2D.isKinematic = true;
				canInterpolate = true;
			}
		}
	}

}

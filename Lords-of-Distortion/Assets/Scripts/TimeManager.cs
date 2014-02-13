using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class TimeManager : MonoBehaviour {

	public static TimeManager instance;
	private float deltaTime;
	public float time;
	
	void Awake(){
		if(instance != null){
			Destroy(this);
			return;
		}
		instance = this;
		DontDestroyOnLoad(this);
	}

	public void SyncTimes(){
		if(Network.isServer){
			deltaTime = -(float)Network.time;
		}
		else{
			print ("Get server time");
			networkView.RPC("GetServerTime", RPCMode.Server);
		}

	}

	void Start(){

	}

	void Update () {
		time = (float)Network.time + deltaTime;
	}

	[RPC]
	void GetServerTime(NetworkMessageInfo info){
		//send to each client that requests
		networkView.RPC("SetDeltaTime", info.sender, time); 
	} 
	
	[RPC]
	void SetDeltaTime (float serverTime, NetworkMessageInfo info)
	{
		deltaTime = serverTime - (float)info.timestamp; 
		Debug.Log("Delta " + deltaTime + "  serverTime =  " + serverTime.ToString()); 
	}
}

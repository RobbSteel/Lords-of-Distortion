using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class TimeManager : MonoBehaviour {

	public static TimeManager instance;
	private float deltaTime;
	public float time;
	
	void Start(){
		if(instance != null){
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(this);
	}

	public void ResetToZero()
	{
		//Only do this once. Calling synctimes again should not modify server time, because we cannot
		//guarantee synchtimes will be called  server first.
		if(Network.isServer){
			//time will start at 0 on server
			deltaTime = -(float)Network.time;
		}
	}

	public void SynchToServer(){
		if(!Network.isServer){
			networkView.RPC("GetServerTime", RPCMode.Server);
		}
	}

	public void UpdateClients()
	{
		if(Network.isServer)
		{
			networkView.RPC("SetDeltaTime", RPCMode.Others, time); 
		}
	}

	void Update () {
		time = (float)Network.time + deltaTime;
	}

	//convert network time value to this synched time
	public float NetworkToSynched(float networkTime){
		return networkTime + deltaTime;
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

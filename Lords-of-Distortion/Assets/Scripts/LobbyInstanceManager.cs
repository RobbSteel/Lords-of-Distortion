using UnityEngine;
using System.Collections.Generic;
using System;

public class LobbyInstanceManager : MonoBehaviour {
	
	int playerCounter;
	//playerID, gameObject
	
	void Awake(){
		playerCounter = -1;
	}
	
	NetworkController myPlayer;
	public Transform characterPrefab;
	public Transform timeManager;
	void SpawnPlayer(NetworkPlayer player)
	{
		Instantiate(timeManager, transform.position, Quaternion.identity);
		string tempPlayerString = player.ToString();
		int playerNumber = Convert.ToInt32(tempPlayerString);
		Transform newPlayerTransform = (Transform)Network.Instantiate(characterPrefab, transform.position, Quaternion.identity, playerNumber);
		myPlayer = newPlayerTransform.GetComponent<NetworkController>();
		NetworkView theNetworkView = newPlayerTransform.networkView;
		theNetworkView.RPC("SetPlayerID", RPCMode.AllBuffered, player);
	}
	
	[RPC] 
	void AddPlayer(){
		playerCounter++;
	}
	
	void OnServerInitialized()
	{
		networkView.RPC("AddPlayer", RPCMode.AllBuffered);
		SpawnPlayer(Network.player);
	}
	
	void OnConnectedToServer()
	{
		networkView.RPC("AddPlayer", RPCMode.AllBuffered);
		SpawnPlayer(Network.player);
	}
	void OnDisconnectedFromServer(){
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
		Network.Destroy(myPlayer.gameObject);
	}
}

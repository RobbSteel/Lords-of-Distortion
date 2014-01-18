using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class LobbyInstanceManager : MonoBehaviour {
	const int GAMEPLAY = 0;
	const int SETUP = 1;
	int playerCounter;
	String offlineLevel = "MainMenu";

	void Awake(){
		DontDestroyOnLoad(this);
		networkView.group = SETUP;
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
		Transform newPlayerTransform = (Transform)Network.Instantiate(characterPrefab, transform.position, Quaternion.identity, GAMEPLAY);
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

		/* Remember to fix this */
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
		Network.Destroy(myPlayer.gameObject);
		//load default offline level
		Application.LoadLevel(offlineLevel);

	}

	[RPC]
	IEnumerator LoadLevel(String level, int levelPrefix){
		Network.SetSendingEnabled(GAMEPLAY, false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		yield return null;
		yield return null;
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(GAMEPLAY, true);
	}
}

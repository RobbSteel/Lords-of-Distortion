using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class LobbyInstanceManager : MonoBehaviour {
	const int GAMEPLAY = 0;
	const int SETUP = 1;
	int playerCounter;
	String offlineLevel = "MainMenu";

	List<PlayerOptions> playerOptions;
	//Initially null until you are connected
	PlayerOptions myPlayerOptions;

	void Awake(){
		DontDestroyOnLoad(this);
		playerOptions = new List<PlayerOptions>();
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
	void RequestLocalSpawn(NetworkMessageInfo info){
		NetworkPlayer original = info.sender;
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered,  ++playerCounter, original);
	}

	[RPC]
	void ConfirmLocalSpawn(int playerNumber, NetworkPlayer original){
		if(original == Network.player){
			myPlayerOptions = new PlayerOptions();
			playerCounter = playerNumber;
			myPlayerOptions.playerNumber = playerNumber;
			Debug.Log (playerCounter);
			SpawnPlayer(Network.player);
		}

		else {
			PlayerOptions other = new PlayerOptions();
			//we can refer to players by number later on
			other.playerNumber = playerNumber;
			playerCounter = playerNumber; //unity guarantees that rpcs will be in order
		}
	}

	void OnServerInitialized()
	{
		myPlayerOptions = new PlayerOptions();
		myPlayerOptions.playerNumber = ++playerCounter;
		//should this be a dictionary?
		playerOptions.Add (myPlayerOptions);
		SpawnPlayer(Network.player);
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered);
	}
	
	void OnConnectedToServer()
	{
		networkView.RPC ("RequestLocalSpawn", RPCMode.Server);
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

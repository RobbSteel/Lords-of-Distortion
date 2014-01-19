using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class LobbyInstanceManager : MonoBehaviour {
	const int GAMEPLAY = 0;
	const int SETUP = 1;
	int playerCounter;
	String offlineLevel = "MainMenu";

	//We want each player to have a list of his own options.
	public Dictionary<NetworkPlayer, PlayerOptions> playerOptions;
	//Initially null until you are connected
	PlayerOptions myPlayerOptions;

	void Awake(){
		DontDestroyOnLoad(this);
		playerOptions = new Dictionary<NetworkPlayer, PlayerOptions>();
		networkView.group = SETUP;
		playerCounter = -1;
	}
	
	NetworkController myPlayer;
	public Transform characterPrefab;
	public Transform timeManager;

	void SpawnPlayer(NetworkPlayer player) 
	{
		Instantiate(timeManager, transform.position, Quaternion.identity);
		//string tempPlayerString = player.ToString();
		///int playerNumber = Convert.ToInt32(tempPlayerString);
		Transform newPlayerTransform = (Transform)Network.Instantiate(characterPrefab, transform.position, Quaternion.identity, GAMEPLAY);
		myPlayer = newPlayerTransform.GetComponent<NetworkController>();
		NetworkView charNetworkView = newPlayerTransform.networkView;
		//we can only really send this after the player has been initialized. Start() in players
		//controller is called late enough and it works, but be careful
		charNetworkView.RPC("SetPlayerID", RPCMode.AllBuffered, player);
	}


	[RPC]
	void RequestLocalSpawn(NetworkMessageInfo info){
		NetworkPlayer original = info.sender;
		++playerCounter;
		ConfirmLocalSpawn (playerCounter, original);
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered,  playerCounter, original);
	}

	[RPC]
	void ConfirmLocalSpawn(int playerNumber, NetworkPlayer original){
		if(original == Network.player){
			PlayerOptions mine = new PlayerOptions();
			playerCounter = playerNumber;
			mine.PlayerNumber = playerNumber;
			Debug.Log ("My spawn " + original +" " +  playerCounter);
			playerOptions.Add(Network.player, mine);
			//do this at the end, so that options are available to player
			SpawnPlayer(Network.player);
		}

		else {
			PlayerOptions other = new PlayerOptions();
			//we can refer to players by number later on
			other.PlayerNumber = playerNumber;
			playerCounter = playerNumber; //unity guarantees that rpcs will be in order
			playerOptions.Add(original, other);
		}
	}

	void OnServerInitialized()
	{
		//these next three lines do same thing as confirmlocalspawn
		myPlayerOptions = new PlayerOptions();
		myPlayerOptions.PlayerNumber = ++playerCounter;
		playerOptions.Add (Network.player, myPlayerOptions);
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered, playerCounter, Network.player);
		SpawnPlayer(Network.player);
	}
	
	void OnConnectedToServer()
	{
		networkView.RPC ("RequestLocalSpawn", RPCMode.Server);
	}

	void OnDisconnectedFromServer(){
		//if(Network.isServer)
		Application.LoadLevel(offlineLevel);
	}

	void OnPlayerDisconnected(NetworkPlayer player){
		/* Remember to fix this */
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		//Network.Destroy(myPlayer.gameObject);
		//load default offline level
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

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class LobbyInstanceManager : MonoBehaviour {

	public bool finishedLoading = false;
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
	
	//NetworkController myPlayer;
	public Transform characterPrefab;
	public Transform timeManager;


	void SpawnPlayerInitial(NetworkPlayer player) 
	{
		Instantiate(timeManager, transform.position, Quaternion.identity);
		//string tempPlayerString = player.ToString();
		///int playerNumber = Convert.ToInt32(tempPlayerString);
		Transform newPlayerTransform = (Transform)Network.Instantiate(characterPrefab, transform.position, Quaternion.identity, GAMEPLAY);
		//myPlayer = newPlayerTransform.GetComponent<NetworkController>();
		NetworkView charNetworkView = newPlayerTransform.networkView;
		//we can only really send this after the player has been initialized. Start() in players
		//controller is called late enough and it works, but be careful
		charNetworkView.RPC("SetPlayerID", RPCMode.AllBuffered, player);
	}

	//This version is called by each when told to by the server.
	[RPC]
	void SpawnPlayer(Vector3 location){
		Transform instance = (Transform)Network.Instantiate(characterPrefab, location, Quaternion.identity, GAMEPLAY);
		NetworkView charNetworkView = instance.networkView;
		//if(Network.isServer)
		//	instance.SendMessage("SetPlayerID", Network.player);

		charNetworkView.RPC("SetPlayerID", RPCMode.All, Network.player);
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
			SpawnPlayerInitial(Network.player);
			
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
		SpawnPlayerInitial(Network.player);
	}
	
	void OnConnectedToServer()
	{
		networkView.RPC ("RequestLocalSpawn", RPCMode.Server);
		//we could also have a custom functions like "Ready" 
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
		finishedLoading = true;

		/* Because we don't want to call network specific functions until we've set the level prefix
		 avoid using Start() in other gameobjects to do networking tasks. Instead call OnNetworkLoadedLevel*/
		GameObject[] objects =  FindObjectsOfType(typeof(GameObject)) as GameObject[];
		foreach(GameObject go in objects)
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);	
	}

	//this function should be called by the server arena manager.
	public void SpawnPlayers(List<Vector3> spawnLocations){
		Dictionary<NetworkPlayer, PlayerOptions>.KeyCollection players = playerOptions.Keys;
		int i = 0;

		foreach(NetworkPlayer player in players){
			if(Network.player == player){
				//this means we the player is the server player
				SpawnPlayer (spawnLocations[i]);
			}
			else
				networkView.RPC ("SpawnPlayer", player, spawnLocations[i]);
			i++;
		}
	}
}

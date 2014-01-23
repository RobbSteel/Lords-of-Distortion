﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class LobbyInstanceManager : MonoBehaviour {

	public bool finishedLoading = false;
	const int GAMEPLAY = 0;
	const int SETUP = 1;
	private int playerCounter;
	public TimeManager timeManager;

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
	public Transform timeManagerPrefab;

	[RPC]
	void RequestLocalSpawn(NetworkMessageInfo info){
		NetworkPlayer original = info.sender;
		++playerCounter;
		ConfirmLocalSpawn (playerCounter, original);
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered,  playerCounter, original);
	}


	//This is called by each client when told to by the server.
	[RPC]
	void SpawnPlayer(Vector3 location){
		Transform instance = (Transform)Network.Instantiate(characterPrefab, location, Quaternion.identity, GAMEPLAY);
		NetworkView charNetworkView = instance.networkView;
		charNetworkView.RPC("SetPlayerID", RPCMode.AllBuffered, Network.player);
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
			SpawnPlayer(transform.position);
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
		Transform instance = (Transform)Instantiate(timeManagerPrefab, transform.position, Quaternion.identity);
		timeManager = instance.GetComponent<TimeManager>();

		//these next three lines do same thing as confirmlocalspawn
		myPlayerOptions = new PlayerOptions();
		myPlayerOptions.PlayerNumber = ++playerCounter;
		playerOptions.Add (Network.player, myPlayerOptions);
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered, playerCounter, Network.player);
		SpawnPlayer(transform.position);
	}
	
	void OnConnectedToServer()
	{
		Transform instance = (Transform)Instantiate(timeManagerPrefab, transform.position, Quaternion.identity);
		timeManager = instance.GetComponent<TimeManager>();
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

		/*Because we don't want to call network specific functions until we've set the level prefix
		 avoid using Start() in other gameobjects to do networking tasks. Instead call OnNetworkLoadedLevel*/
		GameObject[] objects =  FindObjectsOfType(typeof(GameObject)) as GameObject[];
		foreach(GameObject go in objects)
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);	
	}

	//this function should be called by the server arena manager.
	public float SpawnPlayers(List<Vector3> spawnLocations){
		Dictionary<NetworkPlayer, PlayerOptions>.KeyCollection players = playerOptions.Keys;
		int i = 0;

		foreach(NetworkPlayer player in players){
			if(Network.player == player){
				//this means we the player is the server player
				SpawnPlayer (spawnLocations[i]);
			}
			else {
				//Because playeroptions have already been created, we don't do anything special when spawning
				//spawning the arena players.
				networkView.RPC ("SpawnPlayer", player, spawnLocations[i]);
			}
			i++;
		}
		//in 5 seconds begin the round.
		return timeManager.time + 5.0;
	}
}

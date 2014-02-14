using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SessionManager : MonoBehaviour {

	private int levelPrefix; //for networking purposes
	private int arenaIndex; //for loading level purposes.
	private string[] arenas = new string[1]{"StageOne"}; //an array of arenas
	public PSinfo gameInfo;

	public bool finishedLoading = false;
	public const int GAMEPLAY = 0;
	const int SETUP = 1;
	private int playerCounter;
	public bool matchfinish = false;
	public float roundsplayed = 0;
	//public TimeManager timeManager;

	String offlineLevel = "MainMenu";

	public static bool instanceExists = false;
	public static SessionManager instance;


	//Initially null until you are connected
	PlayerOptions myPlayerOptions;
	public TimeManager timemanager;

	void Awake(){
		DontDestroyOnLoad(this);
		instanceExists = true;
		instance = this;
		var information = GameObject.Find("PSInfo");
		gameInfo = information.GetComponent<PSinfo>();
		networkView.group = SETUP;
		playerCounter = -1;
		levelPrefix = 0;
		arenaIndex = -1;// lobby is -1
	}

	//NetworkController myPlayer;
	public Transform characterPrefab;
	public GameObject timeManagerPrefab;

	//The client requests that the server do the following.
	[RPC]
	void RequestLocalSpawn( string username, NetworkMessageInfo info){
		NetworkPlayer original = info.sender;
		++playerCounter;
		ConfirmLocalSpawn (playerCounter, username, original);
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered, playerCounter, username, original);
	}


	//This is called by each client when told to by the server.
	[RPC]
	void SpawnPlayer(Vector3 location){
		//var timeInstance = (GameObject)Instantiate(timeManagerPrefab, transform.position, Quaternion.identity);
		Transform instance = (Transform)Network.Instantiate(characterPrefab, location, Quaternion.identity, GAMEPLAY);
		NetworkView charNetworkView = instance.networkView;
		charNetworkView.RPC("SetPlayerID", RPCMode.AllBuffered, Network.player);
	}

	/*
	 * Create a local copy of PlayerOptions and PlayerStats for each player in this session
	 */

	[RPC]
	void ConfirmLocalSpawn(int playerNumber, string username, NetworkPlayer original){
		playerCounter = playerNumber; //unity guarantees that rpcs will be in order

		PlayerOptions options = new PlayerOptions();
		//we can refer to players by number later on
		options.PlayerNumber = playerNumber;
		gameInfo.playernumb = playerNumber;
		options.username = username; //This is how we know the usernames of other players
		PlayerStats stats = new PlayerStats();
		gameInfo.AddPlayer(original, options, stats);

		if(original == Network.player){
			Debug.Log ("My spawn " + original + " " +  playerCounter);
			//do this at the end so that options are available to the player
			SpawnPlayer(transform.position);
		}
	}

	/*This is the entry point for when the server begins hosting.*/
	void OnServerInitialized()
	{
		timemanager = GameObject.Find ("TimeManager").GetComponent<TimeManager>();
		timemanager.SyncTimes();
		++playerCounter;
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered, playerCounter, gameInfo.playername, Network.player);
		//calling this causes problems because playerID will be set after we spawn, which is too late.
		ConfirmLocalSpawn (playerCounter, gameInfo.playername, Network.player);
		//SpawnPlayer(transform.position);
	}
	
	void OnConnectedToServer()
	{
		timemanager = GameObject.Find ("TimeManager").GetComponent<TimeManager>();
		timemanager.SyncTimes();
		//Instantiate(timeManagerPrefab, transform.position, Quaternion.identity);
		//timeManager = instance.GetComponent<TimeManager>();
		networkView.RPC ("RequestLocalSpawn",  RPCMode.Server, gameInfo.playername);
		//we could also have a custom functions like "Ready" 
	}

	void OnDisconnectedFromServer(){
		//if(Network.isServer)
		Application.LoadLevel(offlineLevel);
		Destroy (TimeManager.instance.gameObject);
		Destroy (this.gameObject);
		Destroy (gameInfo.gameObject);
	}

	void OnPlayerDisconnected(NetworkPlayer player){
		/* Remember to fix this */
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		gameInfo.RemovePlayer(player);
		//Network.Destroy(myPlayer.gameObject);
	}

	[RPC]
	IEnumerator LoadLevel(String level, int commonPrefix){
		Network.SetSendingEnabled(GAMEPLAY, false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(commonPrefix);
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

	//load level now takes a bool to indicate whether the score scene should be loaded.
	public void LoadNextLevel(bool scorescreen){
		string level;

		if(roundsplayed == 3 && scorescreen){
			matchfinish = true;
			roundsplayed = 0;
		}



		if(scorescreen){
				level = "PointsScreen";
		} else {
	    //Right now, because we only have 1 level, we do the same map repeatedly until victory conditions
		arenaIndex = 0;
		roundsplayed++;
		level = arenas[arenaIndex];
		
	}
		//Checks if the match is finished and we're returning from the victory screen so we can go to the lobby
		if(matchfinish && !scorescreen){
			matchfinish = false;
			level = "LobbyArena";
			arenaIndex = -1;
			print ("return to lobby");
		}

		networkView.RPC("LoadLevel", RPCMode.AllBuffered, level, levelPrefix++);
	}

	//this function should be called by the server arena manager.
	public int SpawnPlayers(List<Vector3> spawnLocations){
		int i = 0;
		List<NetworkPlayer> players = gameInfo.players;
		print ("There are " + gameInfo.players.Count);
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
		return players.Count;
	}

	//called when we re-enter this scene after a game is over.
	void OnNetworkLoadedLevel(){
		if(arenaIndex != -1)
			return;
		if(Network.isServer){
			List<Vector3> tempLocations = new List<Vector3>();
			tempLocations.Add(transform.position);
			tempLocations.Add(transform.position);
			tempLocations.Add(transform.position);
			tempLocations.Add(transform.position);
			SpawnPlayers(tempLocations);
		}
	}

	public void KillPlayer(GameObject playerObject){
		networkView.RPC ("Died", RPCMode.OthersBuffered);
		PlayerStats stats = gameInfo.GetPlayerStats(Network.player);
		stats.deaths += 1;
		//Because the player spawned himself, let him destroy himself as well.
		//We may want to instead call a special RPC for an animation or something later on.
		Network.Destroy(playerObject);
	}

	[RPC]
	void Died(NetworkMessageInfo info){
		PlayerStats stats = gameInfo.GetPlayerStats(info.sender);
		stats.deaths += 1;
		Debug.Log (gameInfo.GetPlayerOptions(info.sender).username + " died."); 
	}

	void OnDestroy(){
		instanceExists = false;
	}
}

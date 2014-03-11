using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SessionManager : MonoBehaviour {

	public GameObject DeathSpirit;
	private int levelPrefix; //for networking purposes
	private int arenaIndex; //for loading level purposes.
	private string[] arenas = new string[3]{"StageOne", "StageOne-Two", "StageOne-Three"}; //an array of arenas
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
		NetworkViewID newID = Network.AllocateViewID();
		int group = GAMEPLAY;
		networkView.RPC("InstantiatePlayer", RPCMode.OthersBuffered, location, newID, group);
		Transform instance = InstantiatePlayer(location, newID, group);
		instance.GetComponent<NetworkView>().RPC("SetPlayerID", RPCMode.AllBuffered, Network.player);
	}

	[RPC]
	Transform InstantiatePlayer(Vector3 location, NetworkViewID viewID, int group){
		Transform instance = (Transform)Instantiate(characterPrefab, location, Quaternion.identity);
		NetworkView charNetworkView = instance.GetComponent<NetworkView>();
		charNetworkView.viewID = viewID;
		charNetworkView.group = group;
		return instance;
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
	IEnumerator LoadLevel(String level, int commonPrefix, bool finished){
		matchfinish = finished;
		finishedLoading = false;
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

		if(scorescreen){
			roundsplayed++;
			level = "PointsScreen";
		} else {
			++arenaIndex;
			if(arenaIndex >= arenas.Length){ //we're out of arenas, go back to lobby
				level = "LobbyArena";
				arenaIndex = -1;	
			} else {
				level = arenas[arenaIndex];
			}
		}
		
		if(roundsplayed == 3 && scorescreen){
			print("Match Done");
			matchfinish = true;
			roundsplayed = 0;
			
		}
		
		networkView.RPC("LoadLevel", RPCMode.AllBuffered, level, levelPrefix++, matchfinish);
	}
	
	//this function should be called by the server arena manager.
	public int SpawnPlayers(List<Vector3> spawnLocations){
		int i = 0;
		List<NetworkPlayer> players = gameInfo.players;
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
		if(arenaIndex == -1){
			if(Network.isServer){
				List<Vector3> tempLocations = new List<Vector3>();
				tempLocations.Add(transform.position);
				tempLocations.Add(transform.position);
				tempLocations.Add(transform.position);
				tempLocations.Add(transform.position);
				SpawnPlayers(tempLocations);
			}
		}
	}

	//Called only by the client of the player who died.
	public void KillPlayer(GameObject playerObject){
		print ("My name is " + gameInfo.GetPlayerOptions(Network.player).username);
		networkView.RPC ("Died", RPCMode.Others, Network.player); //Explicitly pass our network player id
		PlayerStats stats = gameInfo.GetPlayerStats(Network.player);
		stats.deaths += 1;
	}
	
	[RPC]
	void Died(NetworkPlayer deadPlayerKey){
		PlayerStats stats = gameInfo.GetPlayerStats(deadPlayerKey);
		stats.deaths += 1;

		GameObject deadPlayer = gameInfo.GetPlayerGameObject(deadPlayerKey);
		//TODO: delay this animation from playing until player reaches spot where he died
		//(using a simple timer and lag calculation)

		//once you learn that a player has died, play his death animation.
		deadPlayer.GetComponent<Controller2D>().anim.SetTrigger("Die");
		Instantiate(DeathSpirit, deadPlayer.transform.position, transform.rotation);
	}
	
	void OnDestroy(){
		instanceExists = false;
	}
}



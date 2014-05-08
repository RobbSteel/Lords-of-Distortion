using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SessionManager : MonoBehaviour {

	public GameObject DeathSpirit;
	private int levelPrefix; //for networking purposes
	private int arenaIndex; //for loading level purposes.
	private string[] arenas = new string[4]{"StageOne", "StageOne-Four", "StageOne-Two", "StageOne-Three"}; //an array of arenas
	public PlayerServerInfo psInfo;
    public LobbyGUI lobbyGUIscript;
	public bool finishedLoading = false;
	public const int GAMEPLAY = 0;
	const int SETUP = 1;
	private int playerCounter;
	public bool matchfinish = false;
	public float roundsplayed = 0;
	//public TimeManager timeManager;
	
	String offlineLevel = "MainMenu";

	public static SessionManager Instance;

	//Initially null until you are connected
	PlayerOptions myPlayerOptions;
	public TimeManager timemanager;
	
	void Awake(){

		if(Instance != null && Instance != this){
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(this);
		var information = GameObject.Find("PSInfo");
		psInfo = information.GetComponent<PlayerServerInfo>();
		networkView.group = SETUP;
		playerCounter = -1;
		levelPrefix = 0;
		arenaIndex = -1;// lobby is -1
	}
	
	//NetworkController myPlayer;
	public Transform colossusPrefab;
	public Transform bluePrefab;

	public GameObject timeManagerPrefab;
	
	//The client requests that the server do the following.
	[RPC]
	void RequestLocalSpawn(string username, int character, NetworkMessageInfo info){
		//re-enable buffered rpc sending
		//Network.SetSendingEnabled(info.sender, SETUP, true);
		NetworkPlayer original = info.sender;
		++playerCounter;
		ConfirmLocalSpawn (playerCounter, username, original, character);
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered, playerCounter, username, original, character);
	}
	
	
	//This is called by each client when told to by the server. 
	//Allocates view ID (thereby becoming owner) and tells others to instantiate character clone
	[RPC]
	void SpawnPlayer(Vector3 location){
		//var timeInstance = (GameObject)Instantiate(timeManagerPrefab, transform.position, Quaternion.identity);
		NetworkViewID newID = Network.AllocateViewID(); //allocate viewId for communications
		int group = GAMEPLAY;
		PlayerOptions localOptions = psInfo.localOptions;
		networkView.RPC("InstantiatePlayer", RPCMode.OthersBuffered, location, (int)localOptions.character, Network.player, newID, group);
		Transform instance = InstantiatePlayer(location, (int)localOptions.character, Network.player, newID, group);
	}

	//Actually instantiates the character game object.
	[RPC]
	Transform InstantiatePlayer(Vector3 location, int character,  NetworkPlayer owner, NetworkViewID viewID, int group){

		Transform instance = null;
		//Instantiate different prefab depending on choice.
		PlayerOptions.Character characterType = (PlayerOptions.Character)character;

		switch(characterType){
		case PlayerOptions.Character.Colossus:
			instance = (Transform)Instantiate(colossusPrefab, location, Quaternion.identity);
			break;
		case PlayerOptions.Character.Blue:
			instance = (Transform)Instantiate(bluePrefab, location, Quaternion.identity);
			break;
		default:
			instance = (Transform)Instantiate(colossusPrefab, location, Quaternion.identity);
			break;
		}

		NetworkView charNetworkView = instance.GetComponent<NetworkView>();
		charNetworkView.viewID = viewID;
		charNetworkView.group = group;

		NetworkController nwController = instance.gameObject.GetComponent<NetworkController>();

		nwController.SetOwner(owner);
		psInfo.AddPlayerGameObject(owner, instance.gameObject);
		SetColor(instance.gameObject, owner);
		return instance;
	}

	void SetColor(GameObject playerObject, NetworkPlayer player){
		PlayerOptions playerOptions = psInfo.GetPlayerOptions(player);
			
		//Used for Ready button
		if(GameObject.Find("LobbyGUI") != null)
		{ 
			LobbyGUI lobbyGuiScript = GameObject.Find("LobbyGUI").GetComponent<LobbyGUI>();
			lobbyGuiScript.SetLocalPlayerNum(playerOptions.PlayerNumber);
			lobbyGuiScript.CreateReadyLight(player);
		}

		SpriteRenderer myRenderer = playerObject.GetComponent<SpriteRenderer>();

		switch(playerOptions.style){
				
		case PlayerOptions.CharacterStyle.BLUE:
			myRenderer.color = Color.blue;
			break;
				
		case PlayerOptions.CharacterStyle.RED:
			myRenderer.color = Color.red;
			break;
				
		case PlayerOptions.CharacterStyle.GREEN:
			myRenderer.color = Color.green;
			break;
		}
	}

	/*
	 * Create a local copy of PlayerOptions and PlayerStats for each player in this session
	 */
	
	[RPC]
	void ConfirmLocalSpawn(int playerNumber, string username, NetworkPlayer owner, int character){
		playerCounter = playerNumber; //unity guarantees that rpcs will be in order
		
		PlayerOptions options = new PlayerOptions();
		//we can refer to players by number later on
		print ("player number is " + playerNumber);
		options.PlayerNumber = playerNumber;
		options.character = (PlayerOptions.Character)character;
		options.username = username; //This is how we know the usernames of other players
		PlayerStats stats = new PlayerStats();
		psInfo.AddPlayer(owner, options, stats);
		
		if(owner == Network.player){
			//do this at the end so that options are available to the player
			SpawnPlayer(transform.position);
		}
	}

	//TODO: Instead of having buffered calls, send rpcs when new player joins.
	/*This is the entry point for when the server begins hosting.*/
	void OnServerInitialized()
	{
		timemanager = TimeManager.instance;
		timemanager.SyncTimes();
		++playerCounter;
		PlayerOptions localOptions = psInfo.localOptions;
		networkView.RPC("ConfirmLocalSpawn", RPCMode.OthersBuffered, playerCounter, localOptions.username,Network.player, (int)localOptions.character);
		//calling this causes problems because playerID will be set after we spawn, which is too late.
		ConfirmLocalSpawn (playerCounter, localOptions.username,  Network.player, (int)localOptions.character);
		//SpawnPlayer(transform.position);
	}
	
	void OnConnectedToServer()
	{
		//Instantiate(timeManagerPrefab, transform.position, Quaternion.identity);
		//timeManager = instance.GetComponent<TimeManager>();
		PlayerOptions localOptions = psInfo.localOptions;
		networkView.RPC ("RequestLocalSpawn",  RPCMode.Server, localOptions.username, (int)localOptions.character);
		timemanager = TimeManager.instance;
		timemanager.SyncTimes();
	}

	void OnPlayerConnected(NetworkPlayer player) {
		//Dont send anything to this player until he loads this stage
		//Network.SetSendingEnabled(player, SETUP, false);
	}

	void OnDisconnectedFromServer(){
		//if(Network.isServer)
		Application.LoadLevel(offlineLevel);
		Destroy (TimeManager.instance.gameObject);
		Destroy (this.gameObject);
		Destroy (psInfo.gameObject);
	}

	[RPC]
	void PlayerDisconnected(NetworkPlayer player){
        if (GameObject.Find("LobbyGUI") != null)
        {
            lobbyGUIscript = GameObject.Find("LobbyGUI").GetComponent<LobbyGUI>();
            lobbyGUIscript.RemoveReadyLight(player);
        }
        
        psInfo.RemovePlayer(player);
		--playerCounter;
    }
	
	void OnPlayerDisconnected(NetworkPlayer player){
		if(psInfo.players.Contains(player)){
			/* Remember to fix this */
            Network.RemoveRPCs(player);
			Network.DestroyPlayerObjects(player);
            PlayerDisconnected(player);
		
			networkView.RPC("PlayerDisconnected", RPCMode.Others, player);
			//Network.Destroy(myPlayer.gameObject);
		}
	}
	
	[RPC]
	IEnumerator LoadLevel(String level, int arenaIndex, int commonPrefix, bool finished){
		this.arenaIndex = arenaIndex;
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

		if(Network.isServer){
			//set update rate back to normal after were in game
			if(arenaIndex == -1)
				MasterServer.updateRate = 60;
		}

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
		
		if(roundsplayed == 4 && scorescreen){
			print("Match Done");
			matchfinish = true;
			roundsplayed = 0;
		}
		
		networkView.RPC("LoadLevel", RPCMode.AllBuffered, level, arenaIndex, levelPrefix++, matchfinish);
	}
	
	//this function should be called by the server arena manager.
	//Returns a copy of the list of live players
	public List<NetworkPlayer> SpawnPlayers(List<Vector3> spawnLocations){
		int i = 0;
		List<NetworkPlayer> players = psInfo.players;
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
		return new List<NetworkPlayer>(players);
	}
	
	//called when we re-enter this scene after a game is over.
	//TODO: fix so that playeroptions are synched.
	void OnNetworkLoadedLevel(){

		if(arenaIndex == -1){
			print("doing it");
			psInfo.ClearPlayers();
			playerCounter = -1;

			if(Network.isServer){
				Network.RemoveRPCsInGroup(SETUP);
				Network.RemoveRPCsInGroup(GAMEPLAY);
				OnServerInitialized();
				/*
				List<Vector3> tempLocations = new List<Vector3>();
				tempLocations.Add(transform.position);
				tempLocations.Add(transform.position);
				tempLocations.Add(transform.position);
				tempLocations.Add(transform.position);
				SpawnPlayers(tempLocations);
				*/
			}
			else{
				OnConnectedToServer();
			}
		}
	}
}



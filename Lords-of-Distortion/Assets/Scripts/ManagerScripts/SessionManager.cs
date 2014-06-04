using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SessionManager : MonoBehaviour {

	public GameObject DeathSpirit;
	private int levelPrefix; //for networking purposes
	private int arenaIndex; //for loading level purposes.
    //TODO: Set this up with arena length from PickManager.cs
	public string[] arenas;// = new string[4]{"empty", "empty", "empty", "empty"}; //an array of arenas
	public PlayerServerInfo psInfo;
    public LobbyGUI lobbyGUIscript;
	public bool finishedLoading = false;
	public const int GAMEPLAY = 0;
	public const int SETUP = 1;
	private int playerCounter;
	public bool matchfinish = false;
	public float roundsplayed = 0;
    private int numberOfRounds;
	//public TimeManager timeManager;
	
	String offlineLevel = "MainMenu";

	public static SessionManager Instance;

	//Initially null until you are connected
	PlayerOptions myPlayerOptions;

	
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
		arenaIndex = -2;// lobby is -1

        numberOfRounds = psInfo.numStages;
        arenas = new string[numberOfRounds];
        for (int i = 0; i < arenas.Length; i++)
        {
            arenas[i] = "empty";
        }
	}
	
	//NetworkController myPlayer;
	public Transform colossusPrefab;
	public Transform bluePrefab;
    public Transform mummyPrefab;

	public GameObject timeManagerPrefab;

	//Called on the server when player sends his info. We can now synchronize this info with all other players.
	//Note this must be called first.
	[RPC]
	void SendOptions(string username, int character, NetworkMessageInfo info){
		//call on server first.
		int playerCount = psInfo.players.Count;
		GeneratePlayerInfo (playerCount, username, info.sender, character);

		//send info about all players to this player
		SendAllPlayerInfo(info.sender);
	}

	//Client generates a viewid for his character and sends to server. Server then sends this to all clients.
	//Should be called second.
	[RPC]
	void SendViewID(NetworkViewID viewID, NetworkMessageInfo info)
	{
		psInfo.AddPlayerViewID(info.sender, viewID); //only the server needs these.
		//We now have all we need to instantiate this player acrsos the network.
		InstantiateAllPlayers(info.sender);
		
		//also instantiate on server. this code doesnt look good here though.
		int characterIndex = (int)psInfo.GetPlayerOptions(info.sender).character;
		InstantiatePlayer(transform.position, characterIndex,  info.sender, viewID, GAMEPLAY);
	}

	//This is called by each client when told to by the server. 
	//Allocates view ID (thereby becoming owner) and tells others to instantiate character clone
	[RPC]
	void SpawnPlayerOld(Vector3 location){
		//var timeInstance = (GameObject)Instantiate(timeManagerPrefab, transform.position, Quaternion.identity);
		NetworkViewID newID = Network.AllocateViewID(); //allocate viewId for communications
		int group = GAMEPLAY;
		PlayerOptions localOptions = psInfo.localOptions;
		networkView.RPC("InstantiatePlayer", RPCMode.Others, location, (int)localOptions.character, Network.player, newID, group);
		GameObject instance = InstantiatePlayer(location, (int)localOptions.character, Network.player, newID, group);
	}

	[RPC]
	void GenerateViewID()
	{
		NetworkViewID newID = Network.AllocateViewID();
		networkView.RPC("SendViewID", RPCMode.Server, newID);
	}


	//Actually instantiates the character game object.
	[RPC]
	GameObject InstantiatePlayer(Vector3 location, int character,  NetworkPlayer owner, NetworkViewID viewID, int group){
		
		//Instantiate different prefab depending on choice.
		Character characterType = (Character)character;
		CharacterStyle palette  = stylesTemp[psInfo.GetPlayerOptions(owner).PlayerNumber];
		
		GameObject characterInstance = GetComponent<CharacterSkins>().GenerateRecolor(characterType, palette);
		characterInstance.transform.position = location;
		characterInstance.SetActive(true);

		 
		NetworkView charNetworkView = characterInstance.GetComponent<NetworkView>();
		charNetworkView.viewID = viewID;
		charNetworkView.group = group;

		NetworkController nwController = characterInstance.GetComponent<NetworkController>();

		nwController.SetOwner(owner);
		psInfo.AddPlayerGameObject(owner, characterInstance);
	
		return characterInstance;
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
				
		case CharacterStyle.BLUE:
			myRenderer.color = Color.blue;
			break;
				
		case CharacterStyle.RED:
			myRenderer.color = Color.red;
			break;
				
		case CharacterStyle.GREEN:
			myRenderer.color = Color.green;
			break;
		}
	}

	/*
	 * Create a local copy of PlayerOptions and PlayerStats for each player in this session.
	 * Also stores in list of connected players.
	 */
	
	[RPC]
	void GeneratePlayerInfo(int playerNumber, string username, NetworkPlayer owner, int character){
		playerCounter = playerNumber; //unity guarantees that rpcs will be in order
		PlayerOptions options = new PlayerOptions();
		//we can refer to players by number later on
		options.PlayerNumber = playerNumber;
		options.character = (Character)character;
		options.username = username; //This is how we know the usernames of other players
		PlayerStats stats = new PlayerStats();
		psInfo.AddPlayer(owner, options, stats);
	}

	/*
	 * For every confirm spawn message, store in buffer. When someone joins, send the rpc
	 * If the player in the buffer DC's, remove from buffer on server.
	 * Basically keeps newly connected players up to date.
	 */
	void SendAllPlayerInfo(NetworkPlayer justJoined)
	{
		//not very reliable, but itll do until we do color logic.
		int playerIndex = 0;
		foreach(NetworkPlayer connectedPlayer in psInfo.players){
			PlayerOptions options = psInfo.GetPlayerOptions(connectedPlayer);
			networkView.RPC("GeneratePlayerInfo", justJoined, playerIndex, options.username, connectedPlayer, (int)options.character);
			playerIndex++;
		}
	}

	//instantiates all players on target client (including his own)
	void InstantiateAllPlayers(NetworkPlayer targetClient)
	{
		foreach(NetworkPlayer ownerOfPlayer in psInfo.players){
			PlayerOptions options = psInfo.GetPlayerOptions(ownerOfPlayer);
			NetworkViewID viewID = psInfo.GetPlayerViewId(ownerOfPlayer);
			networkView.RPC("InstantiatePlayer", targetClient, transform.position, (int)options.character, ownerOfPlayer, viewID, GAMEPLAY);
		}
	}

	CharacterStyle[] stylesTemp = new CharacterStyle[4]{CharacterStyle.GREEN, CharacterStyle.YELLOW, CharacterStyle.BLUE, CharacterStyle.RED}; 
	/*This is the entry point for when the server begins hosting.*/
	void OnServerInitialized()
	{
		TimeManager.instance.ResetToZero();
		playerCounter = 0;
		PlayerOptions localOptions = psInfo.localOptions;
		//the core functionality of sendviewid and sendoptions are done here locally
		//GeneratePlayerInfo (playerCounter, localOptions.username,  Network.player, (int)localOptions.character);
		//TEMPORARY:

		GeneratePlayerInfo (playerCounter, localOptions.username,  Network.player, (int)Character.Colossus);
		NetworkViewID viewID = Network.AllocateViewID();
		psInfo.AddPlayerViewID(Network.player, viewID); 
		//InstantiatePlayer(transform.position, (int)localOptions.character,  Network.player, viewID, GAMEPLAY);
		InstantiatePlayer(transform.position, (int)Character.Colossus,  Network.player, viewID, GAMEPLAY);
	}
	
	void OnConnectedToServer()
	{
		//Instantiate(timeManagerPrefab, transform.position, Quaternion.identity);
		//timeManager = instance.GetComponent<TimeManager>();
		PlayerOptions localOptions = psInfo.localOptions;
		NetworkViewID viewID = Network.AllocateViewID(); //view ids created by clients means they have authority over the network view
		networkView.RPC ("SendOptions", RPCMode.Server, localOptions.username, (int)localOptions.character);
		networkView.RPC("SendViewID", RPCMode.Server, viewID);
		TimeManager.instance.SynchToServer();
	}

	void OnPlayerConnected(NetworkPlayer player) {
		//Dont send anything to this player until he loads this stage
		//Network.SetSendingEnabled(player, SETUP, false);
	}

	void OnDisconnectedFromServer(){
		Destroy (TimeManager.instance.gameObject);
		Destroy (gameObject);
		Destroy (psInfo.gameObject);
		Application.LoadLevel(offlineLevel);
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
            Network.RemoveRPCs(player);
			Network.DestroyPlayerObjects(player);
            PlayerDisconnected(player);
		
			networkView.RPC("PlayerDisconnected", RPCMode.Others, player);
		}
	}
	
	[RPC]
	IEnumerator LoadLevel(String level, int arenaIndex, int commonPrefix, bool finished){
		this.arenaIndex = arenaIndex;
		matchfinish = finished;
		finishedLoading = false;
		Network.SetSendingEnabled(GAMEPLAY, false);

		foreach (NetworkPlayer player in Network.connections) {
			Network.SetReceivingEnabled(player, GAMEPLAY, false);
		}

		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(commonPrefix);
		Application.LoadLevel(level);
		yield return null;
		yield return null;
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(GAMEPLAY, true);

		foreach (NetworkPlayer player in Network.connections) {
			Network.SetReceivingEnabled(player, GAMEPLAY, true);
		}

		finishedLoading = true;
		TimeManager.instance.UpdateClients();
		
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
			if(arenaIndex == -2)
				MasterServer.updateRate = 60;
		}

		string level;

		if(scorescreen){
			roundsplayed++;
			level = "PointsScreen";
		}else if(GameObject.Find("LobbyGUI") != null){

			level = "StageSelect";
			arenaIndex = -1;
		} else {
			++arenaIndex;
			if(arenaIndex >= arenas.Length){ //we're out of arenas, go back to lobby
				level = "LobbyArena";
				arenaIndex = -2;	
			} else {
				level = arenas[arenaIndex];
			}
		}
		
		if(roundsplayed == arenas.Length && scorescreen){
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
				SpawnPlayerOld (spawnLocations[i]);
			}
			else {
				//Because playeroptions have already been created, we don't do anything special when spawning
				//spawning the arena players.
				networkView.RPC ("SpawnPlayerOld", player, spawnLocations[i]);
			}
			i++;
		}
		return new List<NetworkPlayer>(players);
	}
	
	//called when we re-enter this scene after a game is over.
	//TODO: fix so that playeroptions are synched.
	void OnNetworkLoadedLevel(){
		if(arenaIndex == -2){
			psInfo.ClearPlayers();
			playerCounter = -1;

			if(Network.isServer){
				Network.RemoveRPCsInGroup(SETUP);
				Network.RemoveRPCsInGroup(GAMEPLAY);
				OnServerInitialized();
			}
			else{
				OnConnectedToServer();
			}
		}
	}
}



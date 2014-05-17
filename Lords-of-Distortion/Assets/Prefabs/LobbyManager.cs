using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Priority_Queue;

public class LobbyManager : MonoBehaviour {
	PowerPrefabs powerPrefabs;
	
	const float PRE_MATCH_TIME = 5f; 
	const float POST_MATCH_TIME = 5f;
	const float LAST_MAN_TIME = 10f;
	SessionManager sessionManager;
	TrapFountainManager fountainManager; 
	PointTracker pointTracker;
	public List<Transform> spawnPositions;
	private List<Vector3> playerSpawnVectors;
	public List<GameObject> platformlist;
	
	public bool finishgame = false;
	public bool lastman = false;
	
	
	HeapPriorityQueue<PowerSpawn> allTimedSpawns;
	List<NetworkPlayer> playersReady;
	
	float beginTime;
	int? livePlayerCount;
	private bool played;
	bool sentMyPowers = false;
	bool powersSynchronized = false;
	
	private GameObject timer;
	private PowerSpawn prevYield;
	public GameObject alertSymbol;
	
	
	private HUDTools hudTools;
	public GameObject placementRootPrefab;
	PlacementUI placementUI;
	public bool showonce;
	public bool showonce2;
	public bool showonce3;
	public bool showonce4;
	public bool lastmanvictory = false;
	
	List<NetworkPlayer> livePlayers;
	
	enum Phase{
		PreGame,
		InGame,
		FinalPlayer
	}
	Phase currentPhase;
	
	
	[RPC]
	void NotifyBeginTime(float time){
		beginTime = time;
		//set seed of fountain random generator to time
		//fountainManager.SetFirstSpawnTime(beginTime + 15f);
		//fountainManager.SetSeed((int)(beginTime * 1000f));
		//fountainManager.placementUI = placementUI;
	}
	
	
	void ServerDeathHandler(NetworkPlayer player){
		livePlayerCount--;
		livePlayers.Remove(player);
		
		
		if(livePlayerCount == 0){
			print ("No more players");
			//Sets a bool that will be checked by the timer script "Countdown" for game finishing
			finishgame = true;
		}
		
		pointTracker.PlayerDied(player);
		
		if(livePlayerCount == 1 && !finishgame){
			print("Finish him!");
			lastman = true;
			//Get the last live player and pass to point tracker.
			pointTracker.LastManStanding(livePlayers[0]);
		} 
	}
	
	//Tells additional players to destroy clone.
	void NotifyOthersOfDeath(NetworkPlayer deadPlayerID, float timeOfDeath, int deathTypeInteger){
		if(Network.isServer){
			//Store the time of death on server
			float adjustedTime = TimeManager.instance.NetworkToSynched(timeOfDeath);
			
			//Nofify everyone but dead player
			foreach(NetworkPlayer player in SessionManager.Instance.psInfo.players){
				if(player != deadPlayerID && player != Network.player){
					networkView.RPC ("DestroyPlayerClone", player, deadPlayerID, adjustedTime, deathTypeInteger);
				}
			}
			//also destroy player on server
			DestroyPlayerClone(deadPlayerID, adjustedTime, deathTypeInteger);
		}
	}
	
	//Called only on the server. 
	[RPC]
	void NotifyServerOfDeath(int deathTypeInteger, NetworkMessageInfo info){
		//Converts networked message to local function call.
		NotifyOthersOfDeath(info.sender, (float)info.timestamp, deathTypeInteger);
	}
	
	
	//Called on clients not controlling the player who just died.
	[RPC]
	void DestroyPlayerClone(NetworkPlayer deadPlayerID, float timeOfDeath, int deathTypeInteger){
		print ("he dead");
		SessionManager.Instance.psInfo.GetPlayerStats(deadPlayerID).timeOfDeath = timeOfDeath;
		GameObject deadPlayer = SessionManager.Instance.psInfo.GetPlayerGameObject(deadPlayerID);
		deadPlayer.GetComponent<Controller2D>().DieSimple((DeathType)deathTypeInteger);
	}
	
	//Called only on the client where the player died.
	void LostPlayer(GameObject deadPlayer, DeathType deathType, float lives){
		int deathTypeInteger = (int)deathType;
		if(Network.isServer){
			//pass network.time because it's going to be adjusted to synched time anyway
			NotifyOthersOfDeath(Network.player, (float)Network.time, deathTypeInteger);
		}
		
		else {
			SessionManager.Instance.psInfo.GetPlayerStats(Network.player).timeOfDeath = TimeManager.instance.time;
			networkView.RPC ("NotifyServerOfDeath", RPCMode.Server, deathTypeInteger);
		}
		
		//bring up the dead player placement screen.
		//placementUI.SwitchToLive(true);
		//placementUI.enabled = true;
	}

	//Server should do calculations of who to give points to.
	void HandlePlayerEvent(NetworkPlayer affected, PlayerEvent playerEvent){
		print(playerEvent.PowerType + " happened");
		
		if(playerEvent.Attacker != null){
			print ("Attacked by " + playerEvent.Attacker.Value);
			networkView.RPC("SynchEvent", RPCMode.Others, (int)playerEvent.PowerType, 
			                playerEvent.TimeOfContact, playerEvent.Attacker, affected);
		}
		else {
			networkView.RPC("SimpleSynchEvent", RPCMode.Others, (int)playerEvent.PowerType, 
			                playerEvent.TimeOfContact, affected);
		}
		
		//Add locally to server's client
		PlayerStats stats = SessionManager.Instance.psInfo.GetPlayerStats(affected);
		stats.AddEvent(playerEvent);
		
	}
	
	//Synch event with other players, just for the score screen.
	[RPC]
	void SynchEvent(int type, float timeOfContact, NetworkPlayer attacker, NetworkPlayer affected){
		PlayerStats stats = SessionManager.Instance.psInfo.GetPlayerStats(affected);
		PlayerEvent playerEvent = new PlayerEvent((PowerType)type, timeOfContact, attacker);
		stats.AddEvent(playerEvent);
	}
	
	[RPC]
	void SimpleSynchEvent(int type, float timeOfContact, NetworkPlayer affected){
		PlayerStats stats = SessionManager.Instance.psInfo.GetPlayerStats(affected);
		PlayerEvent playerEvent = new PlayerEvent((PowerType)type, timeOfContact);
		stats.AddEvent(playerEvent);
	}
	
	
	//for powers without attacker
	[RPC]
	void SimpleNotifyServerOfEvent(int type, float timeOfContact, NetworkMessageInfo info)
	{
		PlayerEvent playerEvent = new PlayerEvent((PowerType)type, timeOfContact);
		HandlePlayerEvent(info.sender, playerEvent);
	}
	
	[RPC]
	void NotifyServerOfEvent(int type, float timeOfContact, NetworkPlayer attacker, NetworkMessageInfo info){
		PlayerEvent playerEvent = new PlayerEvent((PowerType)type, timeOfContact);
		playerEvent.Attacker = attacker;
		HandlePlayerEvent(info.sender, playerEvent);
	}
	
	//Can be called by affected client or server, so specifing networkplayer is important
	void PlayerEventOccured(NetworkPlayer player, PlayerEvent playerEvent){
		
		if(Network.isServer)
		{
			HandlePlayerEvent(player, playerEvent);
		}
		
		else {
			if(playerEvent.Attacker != null){
				networkView.RPC ("NotifyServerOfEvent", RPCMode.Server, (int)playerEvent.PowerType, playerEvent.TimeOfContact, playerEvent.Attacker);
			}
			
			else{
				networkView.RPC ("SimpleNotifyServerOfEvent", RPCMode.Server, (int)playerEvent.PowerType, playerEvent.TimeOfContact);
			}
		}
	}
	
	/*Clients request this function on the server for every power. After that the server tells every
     other player*/
	
	[RPC]
	void AddPowerSpawnLocally(bool live, int typeIndex, Vector3 position, float time, int localID, NetworkMessageInfo info){
		PowerSpawn requested =  new PowerSpawn(localID);
		requested.type = (PowerType)typeIndex;
		requested.position = position;
		requested.spawnTime = TimeManager.instance.time + time;
		requested.owner = info.sender;
		//print ("Time is set to " + requested.spawnTime)
		allTimedSpawns.Enqueue(requested, requested.spawnTime);
		if(live && Network.isServer){
			networkView.RPC ("AddPowerSpawnLocally", RPCMode.Others, true, (int)requested.type,
			                 requested.position, requested.spawnTime, requested.GetLocalID());
		}
	}
	
	//only to be called by server.
	void AddPowerSpawnLocally(bool live, PowerSpawn requested){
		requested.owner = Network.player;
		allTimedSpawns.Enqueue(requested, requested.spawnTime);
		if(live){
			networkView.RPC ("AddPowerSpawnLocally", RPCMode.Others, true, (int)requested.type,
			                 requested.position, requested.spawnTime, requested.GetLocalID());
		}
	}
	
	
	//Once server has all the spawn info from the other players, send it out.
	private void CheckIfAllSynchronized(){
		if(playersReady.Count == sessionManager.psInfo.players.Count){
			foreach(PowerSpawn power in allTimedSpawns){
				networkView.RPC ("AddPowerSpawnLocally", RPCMode.Others,
				                 false, (int)power.type, power.position, power.spawnTime, power.GetLocalID());
			}
			powersSynchronized = true;
			networkView.RPC("FinishedSettingPowers", RPCMode.Others);
		}
	}
	
	//Called on the server.
	[RPC]
	void SentAllMyPowers(NetworkMessageInfo info){
		playersReady.Add(info.sender);
		CheckIfAllSynchronized();
		print ("Server received power spawn from " + sessionManager.psInfo.GetPlayerOptions(info.sender).username);
	}
	
	void SentAllMyPowers(){
		if(!Network.isServer){
			Debug.Log ("Don't call this function on the client");
			return;
		}
		playersReady.Add(Network.player);
		CheckIfAllSynchronized();
	}
	
	[RPC]
	void FinishedSettingPowers(){
		powersSynchronized = true;
	}
	
	void FindPlatforms(){
		var arenaplatforms = GameObject.FindGameObjectsWithTag("killplatform");
		for(int i = 0; i < arenaplatforms.Length; i++){
			platformlist.Add(arenaplatforms[i]);
		}
	}
	
	void Awake(){
		if(GameObject.Find("LobbyGUI") != null){
			
			sessionManager = SessionManager.Instance;
			FindPlatforms();
			beginTime = float.PositiveInfinity;
			
			playerSpawnVectors = new List<Vector3>();
			
			foreach(Transform location in spawnPositions){
				playerSpawnVectors.Add(location.position);
			}
			hudTools = GetComponent<HUDTools>();
			playersReady = new List<NetworkPlayer>();
			allTimedSpawns = new HeapPriorityQueue<PowerSpawn>(30);
			
			
			powerPrefabs = GetComponent<PowerPrefabs>();
			GameObject placementRoot = Instantiate(placementRootPrefab, 
			                                       placementRootPrefab.transform.position, Quaternion.identity) as GameObject;
			placementUI = placementRoot.GetComponent<PlacementUI>();
			placementUI.Initialize(powerPrefabs);
			placementUI.SwitchToLive(false);
			placementUI.Enable();
			print ("got here");
		} 
	}
	
	// Use this for initialization
	void Start () {
		
		if(GameObject.Find("LobbyGUI") != null){
			
			
		} else {
			
			fountainManager = GameObject.Find("TrapFountainManager").GetComponent<TrapFountainManager>();
			//reset death timers and stuff.
			sessionManager.psInfo.LevelReset();
			
			//this wont print because finishedloading is only true once all the start functions are called
			//in every object of the scene.
			
			if(sessionManager.finishedLoading)
				Debug.Log("ready");
			
		}
	}
	
	
	void OnEnable(){
		//add our function as an event to player
		/*http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
         *http://unity3d.com/learn/tutorials/modules/intermediate/scripting/events*/
		
		Controller2D.onDeath += LostPlayer; //consider making this non static
		PowerSlot.powerKey += SpawnTriggerPower;
		placementUI.spawnNow += SpawnTriggerPower;
	}
	
	
	void OnDisable(){
		Controller2D.onDeath -= LostPlayer;
		PowerSlot.powerKey -= SpawnTriggerPower;
		placementUI.spawnNow -= SpawnTriggerPower;
	}
	/*
       * 1. Load Level Geometry
       * 2. Allow players to place powers. (Spawn instance of Placement UI)
       * 3. Notify players of how much time they have to place powers.
       * 4. When time is over, submit power RPCs to server.
       * 5. Server tells all other players about spawn locations and times. (RPC methods should be as
       * generic as possible, but we'll need different ones if certain powers have extra parameters).
       * 6. Have an event queue similar to the one in NetworkController.
       * 7. Players themselves determine if they're hit or not.* 
	 */
	
	void OnNetworkLoadedLevel(){
		//Instantiate(LordsScreenPrefab, LordsScreenPrefab.position, Quaternion.rotation);
		//TODO: Resynchronize times when level loads, dont draw the timer  ui until this happens
		//TimeManager.instance.SyncTimes();
		if(Network.isServer){
			
			if(GameObject.Find("LobbyGUI") != null){
				print ("lobby");
				currentPhase = Phase.InGame;
				trapsEnabled = true;
			}
		}
		//TODO: have something that checks if all players have finished loading.
	}
	
	private void SpawnTimedTraps(float currentTime){
		if(allTimedSpawns.Count != 0){
			//print ("Current time " + currentTime + ", next trap time " + (beginTime +  allTimedSpawns.First.Priority))
			
			if(currentTime >= allTimedSpawns.First.Priority){
				PowerSpawn spawn = allTimedSpawns.Dequeue();
				//convert power type to an int, which is an index to the array of power prefabs.
				GameObject trap = (GameObject)Instantiate (powerPrefabs.list[(int)spawn.type], spawn.position, Quaternion.identity);
				
				Power powerScript = trap.GetComponent<Power>();
				powerScript.spawnInfo = spawn;
				//One of my own powers just spawned.
				if(spawn.owner == Network.player){
					powerScript.onTrapTrigger += placementUI.DestroyUIPower;
				}
			}
		}
	}
	
	//This is is called when a player presses one of the trigger keys.
	private void SpawnTriggerPower(PowerSpawn spawn, GameObject uiElement){
		float currentTime = TimeManager.instance.time;
		if (placementUI.allTraps.Contains(spawn))
		{
			spawn.owner = Network.player;
			//unitiliazed
			NetworkViewID newViewID = default(NetworkViewID);
			if(spawn.type.PowerRequiresNetworking()){
				//Needs a viewID so that bombs can RPC each other.
				newViewID = Network.AllocateViewID();
			}
			//Call this function locally on and remotely
			networkView.RPC("SpawnPowerLocally", RPCMode.Others, (int)spawn.type, spawn.position, spawn.direction, newViewID,
			                Network.player);
			SpawnPowerLocally(spawn, newViewID);
			//Remove from your inventory and  disable button 
			placementUI.DestroyUIPower(spawn);
			if(uiElement.GetComponent<PowerSlot>() != null){
				PowerSlot powerSlot = uiElement.GetComponent<PowerSlot>();
				powerSlot.enabled = false;
				//remove from grid if power is not infinite.
				if(!powerSlot.associatedPower.infinite)
					placementUI.RemoveFromInventory(powerSlot.associatedPower.type);
				
			}
		}
	}
	
	//http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.StartCoroutine.html
	//http://docs.unity3d.com/Documentation/ScriptReference/Coroutine.html
	//Spawn a warning sign, wait .7 seconds, then spawn power. All of these are done locally on every client.
	IEnumerator YieldThenPower(PowerSpawn spawn, NetworkViewID optionalViewID)
	{
		Vector3 yieldSpawnLocation = spawn.position;
		yieldSpawnLocation.z = -8;
		GameObject instantiatedSymbol = (GameObject)Instantiate(alertSymbol, yieldSpawnLocation, Quaternion.identity);
		yield return new WaitForSeconds(0.7f);
		Destroy(instantiatedSymbol);
		GameObject power =  Instantiate (powerPrefabs.list[(int)spawn.type], spawn.position, Quaternion.identity) as GameObject;;
		power.GetComponent<Power>().spawnInfo = spawn;
		//If the networkview id is specified, apply it to the networkview of the new power
		if(!Equals(optionalViewID, default(NetworkViewID))){
			power.GetComponent<NetworkView>().viewID = optionalViewID;
		}
	}
	
	//this function converts parameters into a powerspawn object
	[RPC]
	void SpawnPowerLocally(int type, Vector3 position, Vector3 direction, NetworkViewID optionalViewID,
	                       NetworkPlayer owner){
		PowerSpawn requestedSpawn = new PowerSpawn();
		requestedSpawn.type = (PowerType)type;
		requestedSpawn.position = position;
		requestedSpawn.direction = direction;
		requestedSpawn.owner  = owner;
		SpawnPowerLocally(requestedSpawn, optionalViewID);
	}
	
	//The function that actually starts the coroutine for spawning powers.
	void SpawnPowerLocally(PowerSpawn spawn, NetworkViewID optionalViewID){
		StartCoroutine(YieldThenPower(spawn, optionalViewID));
	}
	
	bool trapsEnabled = false;
	
	void Update () {
		float currentTime = TimeManager.instance.time;
		//Check to see if you are the last man standing
		if(livePlayerCount == 1 && finishgame && !showonce3){
			networkView.RPC ("LastManVictory", RPCMode.Others);
			lastmanvictory = true;
			hudTools.DisplayText ("Last Player has Survived!");
			showonce3 = true;
		}
		
		if(currentPhase == Phase.PreGame && currentTime >= beginTime){
			
			// placementUI.DisableEditing();
			//placementUI.Disable();
			currentPhase = Phase.InGame;
			hudTools.DisplayText("GO!");
			
			
			placementUI.SwitchToLive(false);
			placementUI.Enable();
			trapsEnabled = true;
		}
		
		if(lastman && !finishgame && !showonce){
			
			hudTools.DisplayText ("Defeat the final player!");
			showonce = true;
		}
		
		if(finishgame && !showonce4 && lastman && !lastmanvictory){
			networkView.RPC("VengeanceMode", RPCMode.Others);
			hudTools.DisplayText ("Vengeance!");
			showonce4 = true;
		}
		
		if(finishgame && !showonce2 && !lastman){
			
			hudTools.DisplayText ("Game Finish!");
			showonce2 = true;
		}
		
		//Spawn one timed trap per frame, locally.
		if(trapsEnabled)
			SpawnTimedTraps(currentTime);
	}
	
	[RPC]
	public void VengeanceMode(){
		hudTools.DisplayText ("Vengeance!");
	}
	
	[RPC]
	public void LastManVictory(){
		this.lastmanvictory = true;
		hudTools.DisplayText("Last Player has Survived!");
	}

	
	
	public float getBeginTime()
	{
		return beginTime;
	}
	
}

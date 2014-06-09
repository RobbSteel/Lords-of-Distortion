using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Priority_Queue;

public class ArenaManager : MonoBehaviour {
	PowerPrefabs powerPrefabs;

	const float PRE_MATCH_TIME = 5f; 
	const float POST_MATCH_TIME = 2f;
	const float LAST_MAN_TIME = 10f;
	SessionManager sessionManager;
	TrapFountainManager fountainManager; 
	PointTracker pointTracker;
	public List<Transform> spawnPositions;
	private List<Vector3> playerSpawnVectors;
	public List<GameObject> platformlist;
	
	HeapPriorityQueue<PowerSpawn> allTimedSpawns;
	List<NetworkPlayer> playersReady;
	
	float beginTime = float.PositiveInfinity;
	float finalPlayerTime = float.PositiveInfinity;
	float endTime = float.PositiveInfinity;


	public LivesUI livesUI;
	int? livePlayerCount;
	private bool played;
	bool sentMyPowers = false;
	bool powersSynchronized = false;
	
	private Timer timer;
	private PowerSpawn prevYield;
	public GameObject alertSymbol;


	private HUDTools hudTools;
	public GameObject placementRootPrefab;
    PlacementUI placementUI;

	string[] messages = new string[4]
	{"Defeat the Last Player!", "Last Player Survived!",  "Game Finish!", "Vengeance!"};

	List<NetworkPlayer> livePlayers;

	//You can set in inspector
	public GameObject RespawnPoint;

	enum Phase{
		PreGame,
		InGame,
		FinalPlayer,
		Finish
	}

	Phase currentPhase;
	//Has the additional task of synching phase

	[RPC]
	void SynchTimer(float time, int phase){
		currentPhase =  (Phase)phase;

		if(currentPhase == Phase.Finish){
			endTime = time;
		}
		else
		{
			SynchUITimer(time);
		}
	}

	private void SynchUITimer(float time){
		timer.countDownTime = time - TimeManager.instance.time;
		timer.Show();
	}

	[RPC]
	void NotifyBeginTime(float time, int playerCount){

		beginTime = time;
		if(time < TimeManager.instance.time)
		{
			Debug.LogError("Network time not synched properly. RPC calls might be getting missed." + (time  - TimeManager.instance.time));
			//Use a temporary fix for incorrect time so that shit don't go crazy
			beginTime = TimeManager.instance.time + PRE_MATCH_TIME;
		}
		
		timer.countDownTime = beginTime - TimeManager.instance.time;
		timer.Show();
		//set seed of fountain random generator to time
		fountainManager.SetFirstSpawnTime(beginTime + 15f - TimeManager.instance.time);
		fountainManager.SetSeed((int)(beginTime * 1000f));
		fountainManager.placementUI = placementUI;
        
		if(playerCount < 3){
			placementUI.Initialize(powerPrefabs);
		}
		else {
			placementUI.Initialize(powerPrefabs, PowerTypeExtensions.RandomActivePower(), PowerType.UNDEFINED);
		}
		placementUI.Disable();
	}

	
	void ServerDeathHandler(NetworkPlayer player, bool disconnect = false, float lives = 0){

		if(lives == 0){
			livePlayerCount--;
			livePlayers.Remove(player);
		}

		//The last player was killed
		if(livePlayerCount == 0){
			FinishGame(false); //Last player didn't win
		}

		if(!disconnect)
		{
			pointTracker.PlayerDied(player);
		}

		if(livePlayerCount == 1 && lives == 0)
		{
			PrintMessage(0); //Defeat final player
			networkView.RPC("PrintMessage", RPCMode.Others, 0);

			//Get the last live player and pass to point tracker.
			pointTracker.LastManStanding(livePlayers[0]);
			currentPhase = Phase.FinalPlayer;

			//give some time to kill last player
			finalPlayerTime = TimeManager.instance.time + LAST_MAN_TIME;
			SynchUITimer(finalPlayerTime);
			networkView.RPC("SynchTimer", RPCMode.Others, finalPlayerTime, (int)Phase.FinalPlayer);
		} 
	}

	//called on server.
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		ServerDeathHandler(player, true, 0);
	}

	//Tells additional players that a player has died.
	void NotifyOthersOfDeath(NetworkPlayer deadPlayerID, float timeOfDeath, int deathTypeInteger){
		if(Network.isServer){
			//Store the time of death on server
			float adjustedTime = TimeManager.instance.NetworkToSynched(timeOfDeath);

			PlayerStats deadPlayerStats = sessionManager.psInfo.GetPlayerStats(deadPlayerID);
			float lives = deadPlayerStats.lives - 1f; //subtract a life
			//Nofify everyone but dead player
			foreach(NetworkPlayer player in sessionManager.psInfo.players){
				if(player != deadPlayerID && player != Network.player){
					networkView.RPC ("PlayerCloneDied", player, deadPlayerID, adjustedTime, deathTypeInteger, lives);
				}
			}
			//also destroy player on server
			PlayerCloneDied(deadPlayerID, adjustedTime, deathTypeInteger, lives);
			//Explicitly pass our network player id
			ServerDeathHandler(deadPlayerID, false, deadPlayerStats.lives);
		}
	}

	//Called only on the server. 
	[RPC]
	void NotifyServerOfDeath(float timeofDeath, int deathTypeInteger, NetworkMessageInfo info){
		//Converts networked message to local function call.
		NotifyOthersOfDeath(info.sender, timeofDeath, deathTypeInteger);
	}


	//Called on clients not controlling the player who just died.
	[RPC]
	void PlayerCloneDied(NetworkPlayer deadPlayerID, float timeOfDeath, int deathTypeInteger, float lives){
		bool canRespawn = true;
		PlayerStats deadPlayerStats = sessionManager.psInfo.GetPlayerStats(deadPlayerID);
		deadPlayerStats.lives = lives;

		if(lives <= 0)
		{
			deadPlayerStats.timeOfDeath = timeOfDeath;
			canRespawn = false;
		}

		if(Network.player != deadPlayerID)
		{ 	//we dont want to call death twice, altough there is already a safeguard
			GameObject deadPlayer = sessionManager.psInfo.GetPlayerGameObject(deadPlayerID);
			deadPlayer.GetComponent<Controller2D>().DieSimple((DeathType)deathTypeInteger, canRespawn);
		}
		
		//update UI
		livesUI.DecreaseLives(deadPlayerID, lives);
	}

	//Called only on the client where the player died.
	void LostPlayer(Controller2D deadPlayer, DeathType deathType){
		int deathTypeInteger = (int)deathType;
	
		if(Network.isServer){
			//pass network.time because it's going to be adjusted to synched time anyway
			NotifyOthersOfDeath(Network.player, TimeManager.instance.time , deathTypeInteger);
		}

		else {
			networkView.RPC ("NotifyServerOfDeath", RPCMode.Server,  deathTypeInteger, TimeManager.instance.time);

			//Do some of the things destroyplayerclone does
			if(deadPlayer.Lives_LOCAL == 0){
				sessionManager.psInfo.GetPlayerStats(Network.player).timeOfDeath = TimeManager.instance.time;
			}

			sessionManager.psInfo.GetPlayerStats(Network.player).lives = deadPlayer.Lives_LOCAL;
			livesUI.DecreaseLives(Network.player, deadPlayer.Lives_LOCAL); //decrease lives in local ui
		}

		if(deadPlayer.Lives_LOCAL == 0){
			//bring up the dead player placement screen.
			placementUI.disabledPowers.Add(PowerType.GATE);
			placementUI.disabledPowers.Add(PowerType.DEFLECTIVE);
			placementUI.SwitchToLive(true);
			placementUI.enabled = true;
		}
	}

	//Server should do calculations of who to give points to.
	void HandlePlayerEvent(NetworkPlayer affected, PlayerEvent playerEvent){
		if(playerEvent.Attacker != null){
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
		print ("Fireball happened at " + timeOfContact);
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
			if(playerEvent.Attacker != null)
			{
				networkView.RPC ("NotifyServerOfEvent", RPCMode.Server, (int)playerEvent.PowerType, playerEvent.TimeOfContact, playerEvent.Attacker);
			}

			else
			{
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

	GameObject[] movingPlatforms;

	void FindPlatforms(){
		GameObject[] arenaplatforms = GameObject.FindGameObjectsWithTag("killplatform");

		for(int i = 0; i < arenaplatforms.Length; i++){
			platformlist.Add(arenaplatforms[i]);
		}

		movingPlatforms = GameObject.FindGameObjectsWithTag("movingPlatform");

		for (int i = 0; i < movingPlatforms.Length; i++)
        {
			platformlist.Add(movingPlatforms[i]);
		}
	}

	void Awake(){

		sessionManager = SessionManager.Instance;
		sessionManager.psInfo.LevelReset();
		
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
		timer = GameObject.Find("timer").GetComponent<Timer>();
		timer.Hide();

		ScoreUI scoreUI = placementRoot.GetComponent<ScoreUI>();
		scoreUI.Initialize(sessionManager.psInfo);

		livesUI = placementRoot.GetComponent<LivesUI>();
		livesUI.Initialize(sessionManager.psInfo, sessionManager.psInfo.livesPerRound);

		pointTracker = GetComponent<PointTracker>();
		pointTracker.Initialize(scoreUI);


	}
	
	// Use this for initialization
	void Start () {

		fountainManager = GameObject.Find("TrapFountainManager").GetComponent<TrapFountainManager>();
		FindPlatforms();
		//if spawn point hasnt been set in inspector, find one
		if(RespawnPoint == null)
			RespawnPoint = GameObject.Find ("Respawn");

		//this wont print because finishedloading is only true once all the start functions are called
		//in every object of the scene.

		if(sessionManager.finishedLoading)
			Debug.Log("ready");
	}


	void OnEnable(){
		//add our function as an event to player
		/*http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
         *http://unity3d.com/learn/tutorials/modules/intermediate/scripting/events*/

		Controller2D.onDeath += LostPlayer; //consider making this non static
		Controller2D.onSpawn += PlayerSpawned;
		PlayerStatus.eventAction += PlayerEventOccured;
		PowerSlot.powerKey += SpawnTriggerPower;
		placementUI.spawnNow += SpawnTriggerPower;
	}

	
	void OnDisable(){
		Controller2D.onDeath -= LostPlayer;
		Controller2D.onSpawn -= PlayerSpawned;
		PlayerStatus.eventAction -= PlayerEventOccured;
		PowerSlot.powerKey -= SpawnTriggerPower;
		placementUI.spawnNow -= SpawnTriggerPower;
	}


	void PlayerSpawned(NetworkPlayer player, Controller2D controller){
		hudTools.ShowPlayerArrow(sessionManager.psInfo.GetPlayerGameObject(player),
		                         sessionManager.psInfo.GetPlayerOptions(player).username);
		//set respawn point
		controller.respawnPoint = RespawnPoint != null ? RespawnPoint.transform.position : Vector3.zero;
		controller.Lives_LOCAL = sessionManager.psInfo.livesPerRound;
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
		if(Network.isServer && GameObject.Find ("PickManager") == null){

			//spawn players immediately. gets
			livePlayers = sessionManager.SpawnPlayers(playerSpawnVectors);
			livePlayerCount = livePlayers.Count;

			NotifyBeginTime(TimeManager.instance.time + PRE_MATCH_TIME, livePlayerCount.Value);
			networkView.RPC ("NotifyBeginTime", RPCMode.Others, beginTime, livePlayerCount.Value);
		}


		hudTools.DisplayText ("Get Ready");
		currentPhase = Phase.PreGame;
		//TODO: have something that checks if all players have finished loading.
	}

	//---Trap Spawning--------------------------------------------
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
		if (placementUI.allTraps.Contains(spawn) && currentTime >= beginTime)
        {
			spawn.owner = Network.player;
			//unitiliazed
			NetworkViewID newViewID = default(NetworkViewID);
			if(spawn.type.PowerRequiresNetworking()){
				//Needs a viewID so that bombs can RPC each other.
				newViewID = Network.AllocateViewID();
			}
			//Call this function locally on and remotely
			networkView.RPC("SpawnPowerLocally", RPCMode.Others, (int)spawn.type, spawn.position, spawn.angle, newViewID,
			                Network.player);
			SpawnPowerLocally(spawn, 0f, newViewID);
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

	//Player sees the warning symbol for his own powers longer than the other players.
	const float MaxWarningDuration = 0.8f; //old one was .7f
	const float MaxLagAdjustment = 0.24f; //The max amount of time the warning can be reduced to account for lag, for fairness.

	//http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.StartCoroutine.html
	//http://docs.unity3d.com/Documentation/ScriptReference/Coroutine.html
	//Spawn a warning sign, wait .7 seconds, then spawn power. All of these are done locally on every client.
	IEnumerator YieldThenPower(PowerSpawn spawn, float warningDuration, NetworkViewID optionalViewID)
    {
        Vector3 yieldSpawnLocation = spawn.position;
        yieldSpawnLocation.z = -8;
		GameObject instantiatedSymbol = (GameObject)Instantiate(alertSymbol, yieldSpawnLocation, Quaternion.identity);
		yield return new WaitForSeconds(warningDuration);
		Destroy(instantiatedSymbol);
		GameObject power =  Instantiate (powerPrefabs.list[(int)spawn.type], spawn.position, Quaternion.identity) as GameObject;
		power.GetComponent<Power>().Initialize(spawn);
		//If the networkview id is specified, apply it to the networkview of the new power
		if(!Equals(optionalViewID, default(NetworkViewID))){
			power.GetComponent<NetworkView>().viewID = optionalViewID;
		}
    }

	//this function converts parameters into a powerspawn object
	[RPC]
	void SpawnPowerLocally(int type, Vector3 position, float angle, NetworkViewID optionalViewID,
	                       NetworkPlayer owner, NetworkMessageInfo info){
		PowerSpawn requestedSpawn = new PowerSpawn();
		requestedSpawn.type = (PowerType)type;
		requestedSpawn.position = position;
		requestedSpawn.angle = angle;
		requestedSpawn.owner  = owner;

		float networkDelay = (float)(Network.time - info.timestamp);
		SpawnPowerLocally(requestedSpawn, networkDelay, optionalViewID);
	}

	//The function that actually starts the coroutine for spawning powers.
	void SpawnPowerLocally(PowerSpawn spawn, float networkDelay,  NetworkViewID optionalViewID){
		float adjustedWarningDuration = MaxWarningDuration - 
			Mathf.Min(networkDelay, MaxLagAdjustment) - (float)NetworkController.interpolationDelay;
		StartCoroutine(YieldThenPower(spawn, adjustedWarningDuration, optionalViewID));
	}

	//--------Trap Spawning End -----------------------
	
	bool trapsEnabled = false;

	void VictoryTaunt(){

		var victor = livePlayers[0];
		var victoryplayer = PlayerServerInfo.instance.GetPlayerGameObject(victor);
		var victorycontroller = victoryplayer.GetComponent<Controller2D>();
		victorycontroller.anim.SetTrigger("Victory");
		victorycontroller.locked = true;
		victorycontroller.powerInvulnerable = true;
		networkView.RPC("VictoryDance", RPCMode.Others, victor);
	}

	[RPC]
	void VictoryDance(NetworkPlayer victor){

			var victoryplayer = PlayerServerInfo.instance.GetPlayerGameObject(victor);
			var victorycontroller = victoryplayer.GetComponent<Controller2D>();
			victorycontroller.anim.SetTrigger("Victory");
			victorycontroller.locked = true;
			victorycontroller.powerInvulnerable = true;

	}

	void Update () {
        float currentTime = TimeManager.instance.time;

		if(currentPhase == Phase.PreGame && currentTime >= beginTime){
			timer.Hide();
			currentPhase = Phase.InGame;
			hudTools.DisplayText("GO!");

			for(int i = 0; i < movingPlatforms.Length; i++){
                if(movingPlatforms[i].GetComponent<movingPlatform>() != null)
                { 
				    movingPlatforms[i].GetComponent<movingPlatform>().enabled = true;
				    if(movingPlatforms[i].GetComponent<FireballSpawner>() != null)
				    {
					    movingPlatforms[i].GetComponent<FireballSpawner>().enabled = true;
				    }
                }
			}
			placementUI.SwitchToLive(false);
			trapsEnabled = true;
		}


		else if(currentPhase == Phase.FinalPlayer && currentTime >= finalPlayerTime){
			if(Network.isServer){
			
				FinishGame(true);
				VictoryTaunt();
			

			}
		}
		else if(currentPhase == Phase.Finish && currentTime >= endTime + 3){
			//game ended, load level
			if(Network.isServer){
				sessionManager.LoadNextLevel(true);
			}

			timer.Hide();
			this.enabled = false;
		}

		//Spawn one timed trap per frame, locally.
		if(trapsEnabled)
			SpawnTimedTraps(currentTime);
	}

	[RPC]
	public void PrintMessage(int message){
		hudTools.DisplayText (messages[message]);
	}
	
	public void FinishGame(bool lastPlayerWon){
		if(Network.isServer){
			if(lastPlayerWon){
				PrintMessage(1);
				networkView.RPC("PrintMessage", RPCMode.Others, 1);
			}
			else{
				PrintMessage(3);
				networkView.RPC("PrintMessage", RPCMode.Others, 3);
			}
			//end game in 3 seconds
			endTime = TimeManager.instance.time + POST_MATCH_TIME;
			networkView.RPC("SynchTimer", RPCMode.Others, endTime, (int)Phase.Finish);
			currentPhase = Phase.Finish;
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerServerInfo {
	
	public string servername;
	public string choice;
	public float livesPerRound;
    public int numStages;

	public PlayerOptions localOptions;

	public HostData chosenHost;
	public int score = 0;


	//We want each player to have a list of his own options.
	public Dictionary<NetworkPlayer, PlayerOptions> playerOptions;
	public Dictionary<NetworkPlayer, PlayerStats> playerStats;
	public List<NetworkPlayer> players;
	public Dictionary<NetworkPlayer, NetworkViewID> playerViewIDs = new Dictionary<NetworkPlayer, NetworkViewID>();
	public Dictionary<NetworkPlayer, GameObject> playerObjects;

	private static readonly PlayerServerInfo instance = new PlayerServerInfo();

	public static PlayerServerInfo Instance
	{
		get
		{
			return instance;
		}
	}

	private PlayerServerInfo()
	{
		localOptions = new PlayerOptions();
		playerOptions = new Dictionary<NetworkPlayer, PlayerOptions>();
		playerStats = new Dictionary<NetworkPlayer, PlayerStats>();
		players= new List<NetworkPlayer>();
		playerObjects = new Dictionary<NetworkPlayer, GameObject>();
	}

	//clear everything but localoptions
	public void ClearInfo()
	{
		choice = "";
		livesPerRound = 0;
		numStages = 0;
		playerOptions.Clear();
		playerStats.Clear();
		players.Clear();
		playerObjects.Clear();
		playerViewIDs.Clear();
	}

	public void LevelReset(){

		foreach(var stats in playerStats){
			stats.Value.LevelReset(livesPerRound);
		}
		//reset lists that should be empty on every level 
		playerObjects = new Dictionary<NetworkPlayer, GameObject>();
		playerViewIDs = new Dictionary<NetworkPlayer, NetworkViewID>();
	}

	public void AddPlayer(NetworkPlayer player, PlayerOptions options, PlayerStats stats){
		players.Add(player);
		playerOptions.Add(player, options);
		playerStats.Add(player, stats);
	}

	public void AddPlayerGameObject(NetworkPlayer player, GameObject gO){
		if(!playerObjects.ContainsKey(player)){
			playerObjects.Add(player, gO);
		}
	}

	public void AddPlayerViewID(NetworkPlayer player, NetworkViewID viewID)
	{
		playerViewIDs.Add(player, viewID);
	}

	public NetworkViewID GetPlayerViewId(NetworkPlayer player)
	{
		if(playerViewIDs.ContainsKey(player))
		{
			return playerViewIDs[player];
		}
		else 
		{
			Debug.LogError("Player not in ViewID list");
			return default(NetworkViewID);
		}
		  
	}

	public GameObject GetPlayerGameObject(NetworkPlayer player){
		GameObject gO = null;
		if(playerObjects.TryGetValue(player, out gO))
			return gO;
		else return null;
	}

	public void RemovePlayer(NetworkPlayer player){
		players.Remove(player);
		playerOptions.Remove(player);
		playerStats.Remove(player);
		playerObjects.Remove(player);
		playerViewIDs.Remove(player);
	}

	public PlayerOptions GetPlayerOptions(NetworkPlayer player){
		PlayerOptions options = null;
		playerOptions.TryGetValue(player, out options);
		return options;
	}

	public PlayerStats GetPlayerStats(NetworkPlayer player){
		PlayerStats stats = null;
		playerStats.TryGetValue(player, out stats);
		return stats;
	}

	public void ClearPlayers(){
		for(int i = players.Count -1; i >= 0; i--){
			RemovePlayer(players[i]);
		}
	}
}

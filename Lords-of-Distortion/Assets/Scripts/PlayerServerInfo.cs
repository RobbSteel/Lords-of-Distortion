using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerServerInfo : MonoBehaviour {
	
	public string playername;
	public string servername;
	public string choice;
	public HostData chosenHost;
	public int score = 0;
	public int playernumb;


	//We want each player to have a list of his own options.
	public Dictionary<NetworkPlayer, PlayerOptions> playerOptions;
	public Dictionary<NetworkPlayer, PlayerStats> playerStats;
	public List<NetworkPlayer> players;
	public Dictionary<NetworkPlayer, GameObject> playerObjects;
	
	void Awake () {
		DontDestroyOnLoad(this);
		playerOptions = new Dictionary<NetworkPlayer, PlayerOptions>();
		playerStats = new Dictionary<NetworkPlayer, PlayerStats>();
		players= new List<NetworkPlayer>();
		playerObjects = new Dictionary<NetworkPlayer, GameObject>();
	}
	public void LevelReset(){
		foreach(var stats in playerStats){
			stats.Value.LevelReset();
		}
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
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PSinfo : MonoBehaviour {
	
	public string playername;
	public string servername;
	public string choice;
	public int servernumb;

	//We want each player to have a list of his own options.
	public Dictionary<NetworkPlayer, PlayerOptions> playerOptions;
	public Dictionary<NetworkPlayer, PlayerStats> playerStats;
	public List<NetworkPlayer> players;

	void Awake () {
		DontDestroyOnLoad(this);
		playerOptions = new Dictionary<NetworkPlayer, PlayerOptions>();
		playerStats = new Dictionary<NetworkPlayer, PlayerStats>();
	}

	public void AddPlayer(NetworkPlayer player, PlayerOptions options, PlayerStats stats){
		players.Add(player);
		playerOptions.Add(player, options);
		playerStats.Add(player, stats);
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

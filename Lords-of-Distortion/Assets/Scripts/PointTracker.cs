using UnityEngine;
using System.Collections;

public class PointTracker : MonoBehaviour{

	PSinfo psInfo;
	public int? livePlayerCount;
	HUDTools hudTools;



	void Awake(){
		SessionManager sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
		psInfo = sessionManager.gameInfo;
		hudTools = GetComponent<HUDTools>();
	}



	public void PlayerDied(NetworkPlayer player){
		
		PlayerStats deadPlayerStats = psInfo.GetPlayerStats(player);
		deadPlayerStats.score += CalculateScore();
		//Tell everyone this player's scores.
		networkView.RPC("SynchronizeScores", RPCMode.Others, deadPlayerStats.score, player);
		
		
		//var managername = gameObject.name;
		//networkView.RPC ("SynchronizePhases", RPCMode.Others, lastman, finishgame, managername);
	}
	
	
	[RPC]
	void SynchronizeScores(int score, NetworkPlayer playerToScore){
		PlayerStats deadPlayerStats = psInfo.GetPlayerStats(playerToScore);
		deadPlayerStats.score = score;
	}

	[RPC]
	void DisplayPoints(int points, NetworkPlayer player){
		//TODO: what if the player who got the points dies before this?
		hudTools.ShowPoints(points, psInfo.GetPlayerGameObject(player));
	}

	//NOTE: should be called from playerdied, make public for testing
	public void GivePoints(int points, NetworkPlayer player){

		networkView.RPC("DisplayPoints", RPCMode.Others, points, player);
		DisplayPoints(points, player);
	}


	//Makes sure that all players are transitioning phases together, such as Lastman standing or game finished
	[RPC]
	void SynchronizePhases(bool lastplayer, bool gamefinal, string managename){
		var tempmanager = GameObject.Find(managename);
		var tempscript = tempmanager.GetComponent<ArenaManager>();
		tempscript.finishgame = gamefinal;
		tempscript.lastman = lastplayer;
		
	}

	//Calculates score based on the number of players remaining when you die, saves it in PSinfo
	public int CalculateScore(){
		
		int score = 0;
		
		if(livePlayerCount == 0){
			
			score += 10;
			
		} else if(livePlayerCount == 1){
			score += 8;
		
		} else if(livePlayerCount == 2){
			
			score += 6;
			
		} else if(livePlayerCount == 3){
			
			score += 4;
		}
		
		return score;
	}


}

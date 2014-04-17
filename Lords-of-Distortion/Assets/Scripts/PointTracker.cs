using UnityEngine;
using System.Collections;
using System.Linq;

public class PointTracker : MonoBehaviour{

	PSInfo psInfo;
	public int? livePlayerCount;
	HUDTools hudTools;



	void Awake(){
		SessionManager sessionManager = SessionManager.Instance;
		psInfo = sessionManager.psInfo;
		hudTools = GetComponent<HUDTools>();
	}


	float timeApart = 2.0f;

	public void PlayerDied(NetworkPlayer player){

		PlayerStats deadPlayerStats = psInfo.GetPlayerStats(player);
		deadPlayerStats.score += CalculateScore();

		//There is guaranteed to be an event that happened before the player died.
		//But there is no guarantee of an attacker killing the player.
		PlayerEvent lastEvent = deadPlayerStats.LastEvent();
		float validTime = lastEvent.TimeOfContact;

		CircularBuffer<PlayerEvent> events = deadPlayerStats.playerEvents;
		//traverse from newest to oldest
		foreach(PlayerEvent playerEvent in events.Reverse<PlayerEvent>()){
			if(playerEvent.TimeOfContact >= validTime - timeApart){

				//Event happened close enough to combo with other event or kill player
				validTime = playerEvent.TimeOfContact;
				NetworkPlayer? attacker = playerEvent.Attacker;
				//Check if the event involved an attacker, so we give points
				if(attacker!= null && attacker.Value != player){
					PlayerStats attackerStats = psInfo.GetPlayerStats(attacker.Value);

					if(attackerStats.timeOfDeath <= playerEvent.TimeOfContact){
						//Attacking player died before the event happened, so give half point.
						GivePoints(0.5f, attacker.Value);
					}
					else {
						//Give full point
						GivePoints(1f, attacker.Value);
					}
				}
			}
		}

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
	void DisplayPoints(float points, NetworkPlayer player){
		//TODO: what if the player who got the points dies before this?
		hudTools.ShowPoints(points, psInfo.GetPlayerGameObject(player));
	}

	//NOTE: should be called from playerdied, make public for testing
	public void GivePoints(float points, NetworkPlayer player){

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

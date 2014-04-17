using UnityEngine;
using System.Collections;
using System.Linq;

public class PointTracker : MonoBehaviour{

	PlayerServerInfo psInfo;
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

		//var managername = gameObject.name;
		//networkView.RPC ("SynchronizePhases", RPCMode.Others, lastman, finishgame, managername);
	}


	[RPC]
	void SynchPoints(float points, NetworkPlayer player){
		PlayerStats playerStats = psInfo.GetPlayerStats(player);
		playerStats.score += points;
		//TODO: what if the player who got the points dies before this?
		hudTools.ShowPoints(points, psInfo.GetPlayerGameObject(player));
	}

	//NOTE: should be called from playerdied, make public for testing
	public void GivePoints(float points, NetworkPlayer player){
		networkView.RPC("SynchPoints", RPCMode.Others, points, player);
		SynchPoints(points, player);
	}


	//Makes sure that all players are transitioning phases together, such as Lastman standing or game finished
	[RPC]
	void SynchronizePhases(bool lastplayer, bool gamefinal, string managename){
		var tempmanager = GameObject.Find(managename);
		var tempscript = tempmanager.GetComponent<ArenaManager>();
		tempscript.finishgame = gamefinal;
		tempscript.lastman = lastplayer;
	}
}

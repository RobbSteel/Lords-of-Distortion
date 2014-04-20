using UnityEngine;
using System.Collections;
using System.Linq;

public class PointTracker : MonoBehaviour{

	PlayerServerInfo psInfo;
	HUDTools hudTools;
	ScoreUI scoreUI;
	
	class PointTimer{
		public float pointDelay = 3f;
		public float timeLeft = 0f;
		public NetworkPlayer player;
		public float pointValue = .5f;

		public PointTimer(NetworkPlayer player, float pointDelay){
			this.player = player;
			this.pointDelay = pointDelay;
			timeLeft = pointDelay;
		}
		
		public void TickTimer(float timePassed){
			timeLeft -= timePassed;
		}
		
		public void ResetTimer(){
			timeLeft = pointDelay;
		}
	}

	PointTimer lastPlayerTimer = null;

	void Awake(){
		SessionManager sessionManager = SessionManager.Instance;
		psInfo = sessionManager.psInfo;
		hudTools = GetComponent<HUDTools>();
	}

	public void Initialize(ScoreUI scoreUI){
		this.scoreUI = scoreUI;
	}

	float timeApart = 2.0f;

	public void PlayerDied(NetworkPlayer player){

		//We've now lost the final player
		if(lastPlayerTimer != null){
			if(player == lastPlayerTimer.player)
				lastPlayerTimer = null;
		}

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


	public void LastManStanding(NetworkPlayer lastMan){ 
		lastPlayerTimer = new PointTimer(lastMan, 3f);
	}

	void Update(){
		//If timer reaches 0, reset and give points to player.
		if(lastPlayerTimer != null){
			lastPlayerTimer.TickTimer(Time.deltaTime);
			if(lastPlayerTimer.timeLeft <= 0){
				lastPlayerTimer.ResetTimer();
				GivePoints(lastPlayerTimer.pointValue, lastPlayerTimer.player);
			}
		}
	}

	[RPC]
	void SynchPoints(float points, NetworkPlayer player){
		PlayerStats playerStats = psInfo.GetPlayerStats(player);
		playerStats.AddToScore(points);

		if(!playerStats.isDead()){
			hudTools.ShowPoints(points, psInfo.GetPlayerGameObject(player));
		}
		scoreUI.IncreasePoints(player);

		//scoreUI.Refresh();

	}

	//NOTE: should be called from playerdied, make public for testing
	void GivePoints(float points, NetworkPlayer player){
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

using System.Collections;


public class PlayerStats {
	public int deaths = 0;
	public int kills = 0;
	public float lives = 0;
	public bool lived = false;
	public float roundScore = 0f;
	public float totalScore = 0f;
	public PlayerEvent lastdeath;
	public float timeOfDeath = float.PositiveInfinity;

	//can only store 3 events
	public CircularBuffer<PlayerEvent> playerEvents =  new CircularBuffer<PlayerEvent>(2);

	public PlayerStats(){

	}

	public void AddToScore(float points){
		roundScore += points;
		totalScore += points;
	}

	public void LevelReset(float liveCount){
		timeOfDeath = float.PositiveInfinity;
		playerEvents = new CircularBuffer<PlayerEvent>(2);
		roundScore = 0f;
		lived = false;
		lives = liveCount;
	}

	public void AddEvent(PlayerEvent playerEvent){
		playerEvents.Add(playerEvent);
	}

	//returns move that killed player usually
	public PlayerEvent LastEvent(){
		lastdeath = playerEvents.ReadNewest();
		return playerEvents.ReadNewest();
	}

	public bool isDead(){
		return timeOfDeath < float.PositiveInfinity;
	}
	
}




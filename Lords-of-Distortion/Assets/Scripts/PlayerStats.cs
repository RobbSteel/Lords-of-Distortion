using System.Collections;


public class PlayerStats {
	public int deaths = 0;
	public int kills = 0;
	public int score = 0;

	public float timeOfDeath = -1f;

	// always stores 3 events
	private CircularBuffer<PlayerEvent> playerEvents =  new CircularBuffer<PlayerEvent>(3);

	public PlayerStats(){

	}

	public void AddEvent(PlayerEvent playerEvent){
		playerEvents.Add(playerEvent);
	}

	//returns move that killed player usually
	public PlayerEvent LastEvent(){
		return playerEvents.ReadNewest();
	}
	
}




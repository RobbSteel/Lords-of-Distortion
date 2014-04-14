using UnityEngine;
using System.Collections;


//METHOD A
//Serialize these and send to server as they happen.
//METHOD B
//Send the last 3 when you die.

//Server may also detect events on its own, in which case the client shouldnt 
//send the same event
public class PlayerEvent{
	
	public float TimeOfContact;
	public PowerType PowerType;
	public NetworkPlayer? Attacker;

	public PlayerEvent(PowerType powerType, float time, NetworkPlayer? attacker = null){
		this.TimeOfContact = time;
		this.PowerType = powerType;
		this.Attacker = attacker;
	}	
}

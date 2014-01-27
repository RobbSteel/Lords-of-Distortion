using UnityEngine;

public class PowerSpawn{

	public enum PowerType{
		STICKY = 0,
		SMOKE, //chalk dust?
		FIREBALL,
		GRAVITY,
		UNDEFINED
	}

	public PowerType type;
	public float spawnTime;
	public Vector3 position;
	public Vector3 direction;

	public PowerSpawn(){
		type = PowerType.UNDEFINED;
	}
}
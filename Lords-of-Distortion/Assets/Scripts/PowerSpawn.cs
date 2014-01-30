using System;
using UnityEngine;
using Priority_Queue;

public class PowerSpawn : PriorityQueueNode {

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
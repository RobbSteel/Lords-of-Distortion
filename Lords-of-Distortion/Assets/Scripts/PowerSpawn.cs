using System;
using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

public enum PowerType{
	STICKY = 0,
	SMOKE, //chalk dust?
	FIREBALL,
	GRAVITY,
	UNDEFINED
}

public class PowerSpawn : PriorityQueueNode {

	public PowerType type;
	public float spawnTime;
	public Vector3 position;
	public Vector3 direction;

	private static List<PowerType> powersRequiringDirection;

	public static bool TypeRequiresDirection(PowerType type){
		return powersRequiringDirection.Contains(type);
	}

	//The static constructor called automatically
	static PowerSpawn(){
		powersRequiringDirection = new List<PowerType>();
		powersRequiringDirection.Add(PowerType.FIREBALL);
	}

	public PowerSpawn(){
		type = PowerType.UNDEFINED;
	}
	
}
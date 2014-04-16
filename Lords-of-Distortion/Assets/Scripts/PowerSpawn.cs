using System;
using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

public enum PowerType{
	STICKY = 0,
	SMOKE, //chalk dust?
	FIREBALL,
	GRAVITY,
	EXPLOSIVE,
	BOULDER,
    FREEZE,
	GATE,
	ELECTRIC,
	PLAGUE,
	EARTH,
	//NON Powers
	MELEE,
	SPIKES,
	HOOK,
	//Special Cases
	POWERHOOK,
	UNDEFINED
}

//http://msdn.microsoft.com/en-us/library/bb383974.aspx
public static class PowerTypeExtensions{

	private static List<PowerType> powersRequiringDirection;
	private static List<PowerType> psuedoPowers = new List<PowerType>();

	public static List<PowerType> powersActive;
	public static List<PowerType> powersPassive;

	public static List<PowerType> powersWithNetworking;


	//The static constructor called automatically
	static PowerTypeExtensions(){
		powersRequiringDirection = new List<PowerType>();
		powersRequiringDirection.Add(PowerType.FIREBALL);
		powersRequiringDirection.Add(PowerType.BOULDER);
		powersRequiringDirection.Add(PowerType.GATE);
		//powersRequiringDirection.Add(PowerType.GRAVITY);
		
		powersActive = new List<PowerType>();
		powersActive.Add(PowerType.FIREBALL);
		powersActive.Add(PowerType.ELECTRIC);
		
		//powersActive.Add(PowerType.EXPLOSIVE);
		//powersActive.Add(PowerType.BOULDER);
		
		powersPassive = new List<PowerType>();
		// powersPassive.Add(PowerType.GRAVITY);
		powersPassive.Add(PowerType.EARTH);
		//powersPassive.Add(PowerType.SMOKE);
		//powersPassive.Add (PowerType.PLAGUE);
		//powersPassive.Add(PowerType.FREEZE);
		//powersPassive.Add(PowerType.GATE);

		psuedoPowers.Add(PowerType.MELEE);
		psuedoPowers.Add(PowerType.SPIKES);
		psuedoPowers.Add(PowerType.HOOK);

		powersWithNetworking.Add(PowerType.FIREBALL);
		powersWithNetworking.Add(PowerType.EXPLOSIVE);
		powersWithNetworking.Add(PowerType.FREEZE);
		powersWithNetworking.Add(PowerType.EARTH);
	}

	public static bool TypeRequiresDirection(this PowerType type){
		return powersRequiringDirection.Contains(type);
	}
	
	public static bool TypeIsActive(this PowerType type){
		return powersActive.Contains(type);
	}
	
	public static bool TypeIsPassive(this PowerType type){
		return powersPassive.Contains(type);
	}

	public static bool IsPsuedoPower(this PowerType type){
		return psuedoPowers.Contains(type);
	}

	public static bool PowerRequiresNetworking(this PowerType type){
		return powersWithNetworking.Contains(type);
	}

}


public class PowerSpawn : PriorityQueueNode {

	public delegate void Timer(PowerSpawn powerSpawn);
	public event Timer timeUpEvent;

	public float timeCountdown;
	public bool timerSet = false;

	public PowerType type;
	public float spawnTime;
	public Vector3 position;
	public Vector3 direction = Vector3.right;
	public NetworkPlayer owner;

	//keeps track of the order in which these powers were spawned.
	private static int powersCreated = 0;

	//Not guaranteed to be unique.
	private int localID;

	public PowerSpawn(){
		type = PowerType.UNDEFINED;
		localID = powersCreated++;
	}

	//returns a copy
	public PowerSpawn(PowerSpawn original){
		type = original.type;
		spawnTime = original.spawnTime;
		position = original.position;
		direction = original.direction;
		owner = original.owner;

		localID = powersCreated++;
	}

	public PowerSpawn(int localID){
		type = PowerType.UNDEFINED;
		this.localID = localID;
	}

	public void SetTimer(float time){
		timerSet = true;
		timeCountdown = time;
	}

	public int GetLocalID(){
		return localID;
	}

	bool calledEvent = false;
	public void ElapseTime(float deltaTime){
		if(timeCountdown > 0f)
			timeCountdown -= deltaTime;
		else if(!calledEvent){
			if(timeUpEvent!= null){
				timeUpEvent(this);
			}
			calledEvent = true;
		}
	}
}
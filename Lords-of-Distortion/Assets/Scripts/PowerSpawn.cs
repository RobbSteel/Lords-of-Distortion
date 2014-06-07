using System;
using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

public enum PowerType{
	STICKY = 0,
	SMOKE,
	FIREBALL,
	GRAVITY,
	EXPLOSIVE,
	BOULDER,
    FREEZE,
	GATE,
	ELECTRIC,
	PLAGUE,
	EARTH,
	BH_OUTER,
	DEFLECTIVE,
	//NON Powers
	MELEE,
	SPIKES,
	HOOK,
	//Special Cases
	POWERHOOK,
	BH_INNER,
	UNDEFINED
}

//http://msdn.microsoft.com/en-us/library/bb383974.aspx
public static class PowerTypeExtensions{

	private static List<PowerType> powersRequiringDirection;
	private static List<PowerType> psuedoPowers = new List<PowerType>();

	public static List<PowerType> powersActive;
	public static List<PowerType> powersPassive;

	public static List<PowerType> powersWithNetworking = new List<PowerType>();


	//The static constructor called automatically
	static PowerTypeExtensions(){
		powersRequiringDirection = new List<PowerType>();
		powersRequiringDirection.Add(PowerType.FIREBALL);
		powersRequiringDirection.Add(PowerType.BOULDER);
		powersRequiringDirection.Add(PowerType.GATE);
		
		powersActive = new List<PowerType>();
		powersActive.Add(PowerType.FIREBALL);
		powersActive.Add(PowerType.ELECTRIC);
		powersActive.Add(PowerType.BH_OUTER);
		powersActive.Add(PowerType.EXPLOSIVE);
		powersActive.Add(PowerType.BOULDER);
        powersActive.Add(PowerType.PLAGUE);


		powersPassive = new List<PowerType>();
		powersPassive.Add(PowerType.GRAVITY);
		powersPassive.Add(PowerType.EARTH);
		//powersPassive.Add(PowerType.SMOKE);
		powersPassive.Add(PowerType.FREEZE);
		powersPassive.Add(PowerType.GATE);
		powersPassive.Add(PowerType.DEFLECTIVE);

		psuedoPowers.Add(PowerType.BH_OUTER);

		powersWithNetworking.Add(PowerType.FIREBALL);
		powersWithNetworking.Add(PowerType.EXPLOSIVE);
		powersWithNetworking.Add(PowerType.FREEZE);
		powersWithNetworking.Add(PowerType.EARTH);
		powersWithNetworking.Add(PowerType.PLAGUE);
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

	public static PowerType RandomActivePower()
	{
		int thisOne = UnityEngine.Random.Range(0, powersActive.Count);
		return powersActive[thisOne];
	}
	
	public static PowerType RandomPassivePower()
	{
		int thisOne = UnityEngine.Random.Range(0, powersPassive.Count);
		return powersPassive[thisOne];
	}
	
	public static PowerType RandomPower()
	{
		int thisOne = UnityEngine.Random.Range(0, 2);
		if (thisOne == 0)
			return RandomActivePower();
		else return RandomPassivePower();
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
	public float angle = 0f;
	public NetworkPlayer owner;
	public bool createEvents = true;

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
		angle = original.angle;
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
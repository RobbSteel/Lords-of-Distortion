﻿using System;
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
	UNDEFINED
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

	private static List<PowerType> powersRequiringDirection;
    public static List<PowerType> powersActive;
    public static List<PowerType> powersPassive;

	public static bool TypeRequiresDirection(PowerType type){
		return powersRequiringDirection.Contains(type);
	}

    public static bool TypeIsActive(PowerType type){
        return powersActive.Contains(type);
    }

    public static bool TypeIsPassive(PowerType type){
        return powersPassive.Contains(type);
    }

	//The static constructor called automatically
	static PowerSpawn(){
		powersRequiringDirection = new List<PowerType>();
		powersRequiringDirection.Add(PowerType.FIREBALL);
		powersRequiringDirection.Add(PowerType.BOULDER);
		powersRequiringDirection.Add(PowerType.GATE);
		//powersRequiringDirection.Add(PowerType.GRAVITY);

        powersActive = new List<PowerType>();
        powersActive.Add(PowerType.FIREBALL);
        //powersActive.Add(PowerType.EXPLOSIVE);
		//powersActive.Add(PowerType.BOULDER);
	
        powersPassive = new List<PowerType>();
        //powersPassive.Add(PowerType.GRAVITY);
       // powersPassive.Add(PowerType.SMOKE);
        powersPassive.Add(PowerType.FREEZE);
		powersPassive.Add(PowerType.GATE);
	}

	public PowerSpawn(){
		type = PowerType.UNDEFINED;
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
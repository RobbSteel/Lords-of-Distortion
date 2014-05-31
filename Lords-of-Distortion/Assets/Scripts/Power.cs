using UnityEngine;
using System.Collections;

public abstract class Power : MonoBehaviour {
	
	public PowerSpawn spawnInfo;
	public bool OFFLINE;

	public delegate void TrapTriggered(Power power);
	public event TrapTriggered onTrapTrigger;

	public virtual void Initialize(PowerSpawn spawn)
	{
		this.spawnInfo = spawn;
	}

	public abstract void PowerActionEnter(GameObject player, Controller2D controller);
	public abstract void PowerActionStay(GameObject player, Controller2D controller);
	public abstract void PowerActionExit(GameObject player, Controller2D controller);

	// function for calling onTrapTrigger
	public void callOnTrapTrigger(){
		if(onTrapTrigger != null){
			onTrapTrigger(this);
			onTrapTrigger = null;
		}
	}
}

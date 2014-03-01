using UnityEngine;
using System.Collections;

public abstract class Power : MonoBehaviour {
	
	public PowerSpawn spawnInfo;

	public delegate void TrapTriggered(Power power);
	public event TrapTriggered onTrapTrigger;

	public abstract void PowerActionEnter(GameObject player, Controller2D controller);
	public abstract void PowerActionStay(GameObject player, Controller2D controller);
	public abstract void PowerActionExit(GameObject player, Controller2D controller);

}

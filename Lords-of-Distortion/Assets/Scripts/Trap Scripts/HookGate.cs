using UnityEngine;
using System.Collections;

public class HookGate : Power {

	void Start(){
		transform.rotation = Quaternion.AngleAxis(spawnInfo.angle, Vector3.forward);
        Destroy(gameObject, 10f);
	}

	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{

	}

	public override void PowerActionStay (GameObject player, Controller2D controller)
	{

	}

	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
			
	}

}

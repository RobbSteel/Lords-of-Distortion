using UnityEngine;
using System.Collections;

public class BlastRadius : Power {

	void Start(){
        //particleSystem.renderer.sortingLayerName = "Foreground";
		Destroy(gameObject, .8f);
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        if(Analytics.Enabled){
			GA.API.Design.NewEvent("Explosion Death", transform.position);
		}

        controller.Die(DeathType.EXPLOSION);
	
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller)
	{
        //controller.Die(DeathType.EXPLOSION);
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller)
	{
		//print("exited");
		//controller.hasbomb = false;
	}



}

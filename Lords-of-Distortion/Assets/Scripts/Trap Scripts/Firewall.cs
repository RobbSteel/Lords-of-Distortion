using UnityEngine;
using System.Collections;

public class Firewall : Power {



	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		


		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Firewall Death", player.transform.position);
		}
		
		controller.Die();
	}


	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}
	
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

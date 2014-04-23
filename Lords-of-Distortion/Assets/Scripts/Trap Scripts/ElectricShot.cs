using UnityEngine;
using System.Collections;

public class ElectricShot : Power {

	private float speed = 3;
	
	
	// Use this for initialization
	void Start () {
		Destroy (gameObject, 6.0f);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Translate((spawnInfo.direction) * (Time.deltaTime * speed)); 
		
	}


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{

		if (GameObject.Find ("CollectData") != null) {
			GA.API.Design.NewEvent ("Electricity Kills", player.transform.position);
		}


        controller.Die(DeathType.FIRE);
	}
	
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}
	
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}




}

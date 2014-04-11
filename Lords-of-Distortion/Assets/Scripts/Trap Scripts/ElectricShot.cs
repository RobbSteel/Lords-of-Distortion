using UnityEngine;
using System.Collections;

public class ElectricShot : Power {
	
	public Vector3 direction;
	private float speed = 3;
	
	
	// Use this for initialization
	void Start () {
		Destroy (gameObject, 6.0f);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Translate((direction) * (Time.deltaTime * speed)); 
		
	}


	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{

		controller.Die();
	}
	
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}
	
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}




}

using UnityEngine;
using System.Collections;

public class SpikeTrapHazard : Power {
	

	private Vector3	startPosition;
	public int velocity = 1;

	void Start(){
		Destroy (gameObject, 5f);
	}

	
	void Awake(){

		//startPosition = transform.position;


	}

	//Sets up spikes tweening from its tween position to its starting position
	private void SetUpTweenPosition(){
	
	}

	//Toggles spikes back and forth for its animation duration 
	//Then 

	void Update(){
	}
/*
	void OnTriggerEnter2D (Collider2D col) 
	{
		// If it hits an enemy...
		if(col.tag == "Player")
		{
			// ... find the Enemy script and call the Hurt function.
			//col.gameObject.GetComponent<Enemy>().Hurt();
			
			// Call the explosion instantiation.
			//OnExplode();
			Debug.Log("Player hit");
			// Destroy the rocket.
			Destroy (gameObject);
		}
		else if (col.tag == "Power") {
		}

		// Otherwise if the player manages to shoot himself...
		else if(col.gameObject.tag != "Player")
		{
			// Instantiate the explosion and destroy the rocket.
			Destroy (gameObject);
		}
	}
*/
	public override void PowerActionEnter(GameObject player, Controller2D controller){
		controller.Die();
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){
		
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){
		
	}
	/*
	public override void PowerActionEnter(GameObject player, Controller2D controller){
		controller.Die();
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller){

	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller){

	}*/
}
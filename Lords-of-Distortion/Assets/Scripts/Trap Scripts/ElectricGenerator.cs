using UnityEngine;
using System.Collections;

public class ElectricGenerator : Power {
	
	
	public GameObject ElectricShot;
	private float time;
	private Vector3 direction;
	private bool plusfire = true;
	private float repetitions = 0;
	// Use this for initialization
	void Start () {
        particleSystem.renderer.sortingLayerName = "Foreground";
		time = 1;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(repetitions == 4){
			
			Destroy(gameObject);
		}
		
		if(time <= 0){
			
			
			if(plusfire){
				
				plusformation();
				plusfire = false;
				repetitions++;
				time = 2;
				
			} else {
				
				xformation();
				plusfire = true;
				repetitions++;
				time = 2;
				
			}
			
			
			
		} else {
			
			time -= Time.deltaTime;
			
			
		}
		
	}

	void CreateShot (Vector3 direction){
		var ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		ElectricShot shot = ElectricityShot.GetComponent<ElectricShot>();
		shot.spawnInfo = new PowerSpawn(this.spawnInfo);
		spawnInfo.direction = direction;
		shot.direction = direction;
	}

	void plusformation(){
		
		//Shot 1 - Up
		CreateShot (new Vector3(0,1,0));
		
		//Shot 2 - Down
		CreateShot (new Vector3(0,-1,0));
		
		//Shot 3 - Left
		CreateShot (new Vector3(-1,0,0));
		
		//Shot 4 - Right
		CreateShot (new Vector3(1,0,0));
		
		
	}
	
	void xformation(){
		
		//Shot 1 - UpperRight
		CreateShot (new Vector3(1, 1, 0));
		
		//Shot 2 - UpperLeft
		CreateShot (new Vector3(-1, 1,0));
		
		//Shot 3 - LowerLeft
		CreateShot (new Vector3(-1,-1,0));
		
		//Shot 4 - LowerRight
		CreateShot (new Vector3(1,-1,0));
		
		
	}
	

	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{
		controller.Die(DeathType.FIRE);
	}
	
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}
	
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}




}
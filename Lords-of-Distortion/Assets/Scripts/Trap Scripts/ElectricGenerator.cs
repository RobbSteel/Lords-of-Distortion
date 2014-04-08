using UnityEngine;
using System.Collections;

public class ElectricGenerator : MonoBehaviour {
	
	
	public GameObject ElectricShot;
	private float time;
	private Vector3 direction;
	private bool plusfire = true;
	private float repetitions = 0;
	// Use this for initialization
	void Start () {
		
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
	
	void plusformation(){
		
		//Shot 1 - Up
		var ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		var shotscript = ElectricityShot.GetComponent<ElectricShot>();
		shotscript.direction = new Vector3(0,1,0);
		
		//Shot 2 - Down
		ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		shotscript = ElectricityShot.GetComponent<ElectricShot>();
		shotscript.direction = new Vector3(0,-1,0);
		
		//Shot 3 - Left
		ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		shotscript = ElectricityShot.GetComponent<ElectricShot>();
		shotscript.direction = new Vector3(-1,0,0);
		
		//Shot 4 - Right
		ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		shotscript = ElectricityShot.GetComponent<ElectricShot>();
		shotscript.direction = new Vector3(1,0,0);
		
		
	}
	
	void xformation(){
		
		//Shot 1 - UpperRight
		var ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		var shotscript = ElectricityShot.GetComponent<ElectricShot>();
		shotscript.direction = new Vector3(1,1,0);
		
		//Shot 2 - UpperLeft
		ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		shotscript = ElectricityShot.GetComponent<ElectricShot>();
		shotscript.direction = new Vector3(-1,1,0);
		
		//Shot 3 - LowerLeft
		ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		shotscript = ElectricityShot.GetComponent<ElectricShot>();
		shotscript.direction = new Vector3(-1,-1,0);
		
		//Shot 4 - LowerRight
		ElectricityShot = (GameObject)Instantiate(ElectricShot, transform.position, transform.rotation);
		shotscript = ElectricityShot.GetComponent<ElectricShot>();
		shotscript.direction = new Vector3(1,-1,0);
		
		
	}
	
	
}
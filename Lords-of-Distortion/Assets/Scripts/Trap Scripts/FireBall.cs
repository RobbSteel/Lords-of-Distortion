﻿using UnityEngine;
using System.Collections;


public class FireBall : Power
{
	public float speed; 
	public AudioClip fireAwake;
	public AudioClip fireLoop;
	public bool redirect = false;


    void Start()
    {
		float angle = Mathf.Atan2(spawnInfo.direction.y, spawnInfo.direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		AudioSource.PlayClipAtPoint( fireAwake , transform.position); 
		AudioSource.PlayClipAtPoint( fireLoop , transform.position); 
        Destroy(gameObject, 10f);
    }
	// Update is called once per frame
    void Update ()
	{
		//Speed should not be dependant on framerate
	
		transform.Translate(Vector3.right * speed * Time.deltaTime);
		
    }

    public override void PowerActionEnter (GameObject player, Controller2D controller)
	{

		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Fireball Death", player.transform.position);
		}
		
		controller.Die();

	}

	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}

	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}

	void OnTriggerEnter2D(Collider2D col){
		print ("at least here");
			print (col.transform);
		if(col.transform.name == "gravityField(Clone)"){
			transform.rotation = Quaternion.AngleAxis(90, new Vector3(0,0,1));

		}

	}

}


	

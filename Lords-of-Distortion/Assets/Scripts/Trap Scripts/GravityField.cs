using UnityEngine;
using System.Collections;

public class GravityField : Power {

	//public float force = 500f;
	public bool triggered; 

	ParticleSystem wind; 

	void Awake(){
		triggered = false;
		wind = GetComponent<ParticleSystem>();
		wind.enableEmission = true;
	}
    void Start()
    {
		float angle = Mathf.Atan2(spawnInfo.direction.y, spawnInfo.direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		wind.enableEmission = false;
		//float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.AngleAxis(angle-90f, Vector3.forward);
		//need to play audio clip here because audio would play during placement mode
		//audio.Play ();
		//if(triggered)
     	 //  Destroy(gameObject, 6f);

    }
	void Update(){

			//Destroy(gameObject);
	}

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
		wind.enableEmission = true;
		triggered = true;

		// calling on Trap Trigger function to destory UIpower when triggered
		callOnTrapTrigger ();

		// CHANGE GRAVITY OF PLAYER
		//player.rigidbody2D.gravityScale = -3;

		player.rigidbody2D.velocity = spawnInfo.direction * 10;
	}

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {

		player.rigidbody2D.velocity = spawnInfo.direction * 10;
	}
    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        //player.rigidbody2D.gravityScale = 1;
    }
  
    void OnDestroy()
    {
        GameObject user = GameObject.FindGameObjectWithTag("Player");
        if (user != null)
            user.rigidbody2D.gravityScale = 1;
    }
}

using UnityEngine;
using System.Collections;

public class GravityField : Power {

	//public float force = 500f;
	public bool triggered;
    private GameObject playerGO;

	ParticleSystem wind; 

	void Awake(){
		//triggered = false;
		wind = GetComponent<ParticleSystem>();
		wind.enableEmission = true;
	}
    void Start()
    {
		//wind.enableEmission = false;
		//float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.AngleAxis(angle-90f, Vector3.forward);
		//need to play audio clip here because audio would play during placement mode
		audio.Play ();
		//if(triggered)
     	Destroy(gameObject, 15f);

    }
	void Update(){
		//if(triggered)
			//Destroy(gameObject, 6f);
	}

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
		wind.enableEmission = true;
		//triggered = true;

		// calling on Trap Trigger function to destory UIpower when triggered
		//callOnTrapTrigger ();
		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Gravity Touches", player.transform.position);

		}
		// CHANGE GRAVITY OF PLAYER
        player.rigidbody2D.gravityScale = -3;
        playerGO = player;
	}

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
		//Debug.Log (transform.up);
		//player.rigidbody2D.AddForce(transform.up * force);
	}

    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.gravityScale = 1;
    }
  
    void OnDestroy()
    {
        if (playerGO != null)
            playerGO.rigidbody2D.gravityScale = 1.8f;
    }
}

using UnityEngine;
using System.Collections;

public class GravityFieldStage : Power {

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
     	//Destroy(gameObject, 15f);
        Destroy(GetComponent<GravityFieldIndicators>());
    }

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
		if(Analytics.Enabled){
			GA.API.Design.NewEvent("Gravity Touches", player.transform.position);

		}
		// CHANGE GRAVITY OF PLAYER
        player.rigidbody2D.gravityScale = -3;
        playerGO = player;
	}

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
		player.rigidbody2D.gravityScale = -3;
		playerGO = player;
	}

    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.gravityScale = 1;
		playerGO = null;
    }
  
    void OnDestroy()
    {
        if (playerGO != null)
            playerGO.rigidbody2D.gravityScale = 1.8f;
    }
}

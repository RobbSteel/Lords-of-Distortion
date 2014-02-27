using UnityEngine;
using System.Collections;

public class GravityField : Power {
	public float force = 500f;
	public GameObject windObject;
	public bool triggered = false;
	ParticleSystem wind; 

	void Awake(){
		wind = GetComponent<ParticleSystem>();
		wind.enableEmission = true;
		//windObject.GetComponent<Sprite>().enabled = false;

	}
    void Start()
    {
		wind.enableEmission = false;

		//float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.AngleAxis(angle-90f, Vector3.forward);
		//need to play audio clip here because audio would play during placement mode
		//audio.Play ();
		//if(triggered)
     	  // Destroy(gameObject, 6f);

    }
	void Update(){
		if(triggered)
			Destroy(gameObject, 6f);
	}

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
		wind.enableEmission = true;
		wind.Play();

		player.rigidbody2D.gravityScale = -3;
		//windObject.GetComponent<Sprite>().enabled = true;

		//player.rigidbody2D.AddForce(this.direction * force);
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
        //Debug.Log("Destroyed");
        GameObject user = GameObject.FindGameObjectWithTag("Player");
        if (user != null)
            user.rigidbody2D.gravityScale = 1;
    }
}

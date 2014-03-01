using UnityEngine;
using System.Collections;

public class GravityField : Power {

    void Start()
    {
		//gameObject.transform.rotation = new Quaternion(0,0,-90);

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		//need to play audio clip here because audio would play during placement mode
		//audio.Play ();
        //Destroy(gameObject, 6f);
    }

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
		player.rigidbody2D.AddForce(new Vector2(0,100f));
    }

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
		player.rigidbody2D.AddForce (new Vector2(0,100f));
    }

    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        
    }


  
    void OnDestroy()
    {
        //Debug.Log("Destroyed");
        GameObject user = GameObject.FindGameObjectWithTag("Player");
        if (user != null)
            user.rigidbody2D.gravityScale = 1;
    }
}

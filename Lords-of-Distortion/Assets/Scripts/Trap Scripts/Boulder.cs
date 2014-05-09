using UnityEngine;
using System.Collections;

public class Boulder : Power {

    public GameObject rockShatterPrefab;
	float direction = -1.0f;

	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{

		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Boulder Death", player.transform.position);
		}

		controller.Die(DeathType.CRUSH);
	}
	public override void PowerActionStay (GameObject player, Controller2D controller)
	{

	}
	public override void PowerActionExit (GameObject player, Controller2D controller)
	{

	}

    void OnDestroy()
    {
        if (gameObject.tag == "Power")
        {
            GameObject rockShatter = Instantiate(rockShatterPrefab, transform.position, Quaternion.identity) as GameObject;
        }
    }

	Vector2 force = new Vector2(-200f, 50);
	Vector2 velocity = new Vector2(8f, 0f); 

	// Use this for initialization
	void Start () {

		//move boulder either right or left.
		if(spawnInfo.direction.x > 0f){
			direction = 1.0f;
		}

		force.x = force.x * direction;
		velocity.x = velocity.x * direction;
        Destroy(gameObject, 10f);
	}


	// Update is called once per frame
	void FixedUpdate () {
		//rigidbody2D.AddForce(force);
		rigidbody2D.velocity = new Vector2(velocity.x, rigidbody2D.velocity.y);
	}
	
    void OnCollisionEnter2D(Collision2D collider)
    {
        Debug.Log("COLLISION WITH " + collider);
        if(collider.gameObject.tag == "SolidObject" || collider.gameObject.name == gameObject.name)
        {
            /*      Reverse Direction
             * direction = -1 * direction;
             * force.x = force.x * direction;
             * velocity.x = velocity.x * direction;
             * rigidbody2D.velocity = new Vector2(velocity.x, rigidbody2D.velocity.y);
             */
            Destroy(gameObject);
        }
    }

}

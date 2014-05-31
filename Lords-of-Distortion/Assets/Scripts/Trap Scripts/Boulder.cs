using UnityEngine;
using System.Collections;

public class Boulder : Power {

    public GameObject rockShatterPrefab;
	float direction = 1.0f;

	public override void PowerActionEnter (GameObject player, Controller2D controller)
	{

		if(Analytics.Enabled){
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
	
	Vector2 velocity = new Vector2(7f, 0f); 

	// Use this for initialization
	void Start () {

		transform.rotation = Quaternion.AngleAxis(spawnInfo.angle, Vector3.forward);
		//clamp angle to a direction.
		if(transform.eulerAngles.z >= 90f &&  transform.eulerAngles.z <= 270f)
		{
			direction = -1f;
		}

		velocity.x = velocity.x * direction;
        Destroy(gameObject, 10f);
	}


	// Update is called once per frame
	void FixedUpdate () {
		rigidbody2D.velocity = new Vector2(velocity.x, rigidbody2D.velocity.y);
	}
	
    void OnCollisionEnter2D(Collision2D collider)
    {
        if(collider.gameObject.tag == "SolidObject" || collider.gameObject.name == gameObject.name)
        {
            Destroy(gameObject);
        }
    }

}

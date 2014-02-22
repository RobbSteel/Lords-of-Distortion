using UnityEngine;
using System.Collections;

public class FireBall : Power
{
	public float speed; 

    void Start()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		//audio.Play();
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
        controller.Die();
	}

	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}

	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}
}


	

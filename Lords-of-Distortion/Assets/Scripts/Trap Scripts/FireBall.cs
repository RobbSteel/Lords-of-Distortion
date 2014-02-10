using UnityEngine;
using System.Collections;

public class FireBall : Power
{
	public float speed; 

	// Update is called once per frame
    void Update ()
	{
		//Speed should not be dependant on framerate
		transform.Translate(direction * speed * Time.deltaTime);
		//transform.up = direction;
		//float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		//transform.up = direction;

		/*
        if (!aiming)// && mouseClicks == 2)
        { 
            var mousePos = Input.mousePosition;
            mousePos.z = 10.0f; //The distance from the camera to the player object

            direction = Camera.main.ScreenToWorldPoint(mousePos);
            //Debug.Log("Fireball : " + direction);
            mouseClicks++;
            //transform.Translate(speed, 0, 0);
        }

        if (!aiming)// && mouseClicks == 3)
            transform.Translate(speed, 0, 0);

        if (aiming)// && mouseClicks == 1)
        {
            var mousePos = Input.mousePosition;

            mousePos.z = 10.0f; //The distance from the camera to the player object
            Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
            lookPos = lookPos - transform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (Input.GetMouseButtonUp(0) == true)
            {
                mouseClicks++;
                aiming = false;
            }
        }/*
        if (Input.GetMouseButtonUp(0) == true && mouseClicks == 0)
        {
            //aiming = false;
            mouseClicks++;
            speed = .1f;
            fireball = (GameObject)Instantiate(Resources.Load("fireball"));
            
            Destroy(fireball);
        }*/
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


	

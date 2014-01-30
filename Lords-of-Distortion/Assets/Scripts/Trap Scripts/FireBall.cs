using UnityEngine;
using System.Collections;

public class FireBall : Power
{
    bool aiming;
	public float speed = 0.1f;
    private GameObject fireball;
    public int mouseClicks = 0;
    public Vector3 direction = new Vector3(0,0,0);

	void Start ()
	{
        aiming = false;
	}
    

	// Update is called once per frame
    void Update ()
	{
        if (!aiming)// && mouseClicks == 2)
        { 
            var mousePos = Input.mousePosition;
            mousePos.z = 10.0f; //The distance from the camera to the player object

            direction = Camera.main.ScreenToWorldPoint(mousePos);
            Debug.Log("Fireball : " + direction);
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
	    Destroy (player);
	}

	public override void PowerActionStay (GameObject player, Controller2D controller)
	{
	}

	public override void PowerActionExit (GameObject player, Controller2D controller)
	{
	}
}


	

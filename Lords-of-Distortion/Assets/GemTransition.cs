using UnityEngine;
using System.Collections;

public class GemTransition : MonoBehaviour {

    private int gemHealth = 100;
    private bool transition = false;
    private bool broken = false;
    private float timeBetweenDamage = 3f;

    public GameObject Part1;
    public GameObject Part2;

    //private CameraShake shake;
    public Camera mainCam;
	
    void OnStart()
    {
      //  shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

	// Update is called once per frame
	void Update () 
    {
        if (timeBetweenDamage > 0)
        {
            timeBetweenDamage -= Time.deltaTime;
        }

	    if(gemHealth <= 0)
        {
            Debug.Log("GEM DEAD");
            transition = true;
        }

        if (transition && !broken)
        {
            Part1.SetActive(false);
            Part2.SetActive(true);
            mainCam.GetComponent<CameraShake>().PlayShake();
            broken = true;
            //shake.PlayShake();
        }

	}

    void ExplosionCollision(Collider2D collider)
    {
        Debug.Log("TESTING LOWER COLLISION: " + collider.name);
        GameObject explosion = GameObject.Find("BombExplosion(Clone)");
        if (explosion != null)
        {
            if (collider.transform.name == explosion.name && explosion.tag == "Power")
            {
                gemHealth -= 50;
            }
        }
    }

    void EarthquakeCollision(Collider2D collider)
    {
        GameObject earthquake = GameObject.Find("Earthquake(Clone)");
        if (earthquake != null)
        {
            if (collider.transform.name == earthquake.name && earthquake.tag == "Power")
            {
                gemHealth -= 25;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (timeBetweenDamage <= 0) 
        { 
            switch(collider.name)
            {
                case "Earthquake(Clone)" : EarthquakeCollision(collider); break;
                case "BombExplosion(Clone)": ExplosionCollision(collider); break;
            }

            timeBetweenDamage = 3f;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (timeBetweenDamage <= 0)
        {
            Debug.Log(collider.name + " Collided with the GEM");

            switch (collider.name)
            {
                case "Earthquake(Clone)": EarthquakeCollision(collider); break;
                case "BombExplosion(Clone)": ExplosionCollision(collider); break;
            }

            timeBetweenDamage = 3f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Boulder(Clone)")
        {
            gemHealth -= 20;
        }
    }


}

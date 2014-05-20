using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemTransition : MonoBehaviour {

    private int gemHealth = 100;
    private bool transition = false;
    private bool broken = false;
    private float timeBetweenDamage = 3f;
    private float timer = 0f;
    //private int callOnce = 0;

    public ParticleSystem earthShatter;
    public GemShatterLighting gemLighting;
    public GameObject Part1;
    public GameObject Part2;
    public PlayerServerInfo psinfo;

    //private CameraShake shake;
    public Camera mainCam;

    enum StagePhase
    {
        GemAlive = 0,
        GemBroken = 1,
        GemDestroyed = 2
    };

    private StagePhase currentPhase = StagePhase.GemAlive;

    void Awake()
    {
        psinfo = GameObject.Find("PSInfo").GetComponent<PlayerServerInfo>();
        Part2.SetActive(false);
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

            timer += Time.deltaTime;
            LiftPlayers();

            if (timer > 4)
            {
                transition = true;
            }

            if (currentPhase == StagePhase.GemAlive)
            {
                currentPhase = StagePhase.GemBroken;
            }
            if (currentPhase == StagePhase.GemBroken)
            {
                currentPhase = StagePhase.GemDestroyed;
                gemLighting.GemShattered(true);
                DestroyPlatform();
                DestroyCharmarks();
                mainCam.GetComponent<CameraShake>().PlayShake();
            }
        }

        if (transition && !broken)
        {
            LowerPlayers();
            Part1.SetActive(false);
            Part2.SetActive(true);
            broken = true;
            gemLighting.GemShattered(false);
            gemLighting.ResetLighting();
            gameObject.SetActive(false);
        }

	}

    void LiftPlayers()
    {
        foreach(NetworkPlayer player in psinfo.players)
        {
            if(player != null)
            { 
                GameObject playerGO = psinfo.GetPlayerGameObject(player);
                if(playerGO != null)
                { 
                    playerGO.GetComponent<Rigidbody2D>().gravityScale = -0.25f;
                    playerGO.GetComponent<Controller2D>().locked = true;
                }
            }
        }
    }

    void LowerPlayers()
    {
        foreach (NetworkPlayer player in psinfo.players)
        {
            if(player != null)
            { 
                GameObject playerGO = psinfo.GetPlayerGameObject(player);
                if(playerGO != null)
                { 
                    playerGO.GetComponent<Rigidbody2D>().gravityScale = 1;
                    playerGO.GetComponent<Controller2D>().locked = false;
                }
            }
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
                gemHealth -= 100; //50
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
                gemHealth -= 100; //25
            }
        }
    }

    void DestroyCharmarks()
    {
        GameObject[] charmarks = GameObject.FindGameObjectsWithTag("charmark");
        foreach(GameObject mark in charmarks)
        {
            Destroy(mark);
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

            timeBetweenDamage = 1f;
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

            timeBetweenDamage = 1f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Boulder(Clone)")
        {
            gemHealth -= 100;// 20;
        }
    }

    [RPC]
    void CrackPlatform(int index)
    {
        var manager = GameObject.FindGameObjectWithTag("ArenaManager");
        var managerscript = manager.GetComponent<ArenaManager>();
        var platformlist = managerscript.platformlist;
        var platform = platformlist[index];
        Instantiate(earthShatter, platform.transform.position, platform.transform.rotation);
        Destroy(platform);
    }

    void DestroyPlatform()
    {
        if (Network.isServer)
        {
            int index = 0;
            GameObject manager = GameObject.FindGameObjectWithTag("ArenaManager");
            ArenaManager managerscript = manager.GetComponent<ArenaManager>();
            List<GameObject> platformlist = managerscript.platformlist;

            foreach(GameObject go in platformlist)
            {
                if (go != null && go.name == "Part1Platform")
                {
                    Instantiate(earthShatter, go.transform.position, go.transform.rotation);
                    networkView.RPC("CrackPlatform", RPCMode.Others, index);
                    Destroy(go);
                }

                index += 1;
            }
        }
    }

}

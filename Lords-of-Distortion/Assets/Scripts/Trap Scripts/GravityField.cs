using UnityEngine;
using System.Collections;

public class GravityField : Power {

    private GameObject gravField;
    public float timer = 0;
    public bool prepareSpawn = false;
    public bool fieldDestroyed = true;

	// Update is called once per frame
	void Update () {
        GravField();
	}

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.gravityScale = -1;
    }

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {

    }

    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.gravityScale = 1;
    }
	public override void PowerActionStay(GameObject player, Controller2D controller){
		}

  

    public void GravField()
    {
        if (Input.GetKeyUp(KeyCode.G) == true && fieldDestroyed == true)
        {
            prepareSpawn = true;    
        }

        if (prepareSpawn == true && Input.GetMouseButtonUp(0))
        {
            fieldDestroyed = false;
            prepareSpawn = false;
            Vector3 mouseClick = Input.mousePosition;
            mouseClick = Camera.main.ScreenToWorldPoint(mouseClick);

            // Local Spawn
            //GameObject gravField = (GameObject)Instantiate(Resources.Load("gravityField"));

            timer = Time.time;

            // Network Spawn - Not sure if this works, fails because "We are not connected"
            gravField = (GameObject)Network.Instantiate(Resources.Load("gravityField"), mouseClick, Quaternion.identity, -1);
        }

        if (Time.time - timer > 5)
        {
            fieldDestroyed = true;
            Network.Destroy(gravField);
        }
    }

    void OnDestroy()
    {
        Debug.Log("Destroyed");
        GameObject user = GameObject.FindGameObjectWithTag("Player");
        if (user != null)
            user.rigidbody2D.gravityScale = 1;
    }
}

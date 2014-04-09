using UnityEngine;
using System.Collections;

public class PlagueScript : Power 
{
    private float timer = 0f;
	private float timeTilDrag = 1f;
    private float dragGiven = 5f;
    private GameObject playerGO;

    void Start () 
    {
        Destroy(gameObject, 15f);
	}
	
	void Update ()
    {
	}

    void OnDestroy()
    {
        if(playerGO != null)
            playerGO.rigidbody2D.drag = 0;
    }

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        if (player.rigidbody2D.drag == 0)
        {
            player.rigidbody2D.drag += 15;
            playerGO = player;
        }
    }

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
        timer += Time.deltaTime;

        if (timer > timeTilDrag)
        {
            player.rigidbody2D.drag += 10;
            timer = 0;
            
            if (playerGO.rigidbody2D.drag > 50)
            {
                controller.Die();
            }
        }
    }

    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.drag = 0;
    }


}

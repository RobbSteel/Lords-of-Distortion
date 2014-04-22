using UnityEngine;
using System.Collections;

public class PlagueScript : Power 
{
    private float timer = 0f;
	private float timeTilDrag = 1f;
    private float dragGiven = 5f;
    //private PlayerStatus status;
    private GameObject playerG0;
    void Start () 
    {
        Destroy(gameObject, 15f);
	}

    void OnDestroy()
    {
        //if(status != null)
        //    status.insidePlague = false;
        if (playerG0 != null)
            playerG0.rigidbody2D.drag = 0;
    }

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        //status = player.GetComponent<PlayerStatus>();
        //status.insidePlague = true;
        playerG0 = player;
        if (player.rigidbody2D.drag == 0)
        {
            player.rigidbody2D.drag += 25;
        }
    }

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
        timer += Time.deltaTime;
        //status.insidePlague = true;
        if (timer > timeTilDrag)
        {
            player.rigidbody2D.drag += 12f;
            timer = 0;
            
            if (player.rigidbody2D.drag > 50)
            {
				if (GameObject.Find ("CollectData") != null) {
					GA.API.Design.NewEvent ("Plague Deaths", player.transform.position);
				}
				controller.Die(DeathType.PLAGUE);
            }
        }
    }

    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.drag = 0;
        //status.insidePlague = false;
    }
}

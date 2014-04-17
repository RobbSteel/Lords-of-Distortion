using UnityEngine;
using System.Collections;

public class PlagueScript : Power 
{
    private float timer = 0f;
	private float timeTilDrag = 1f;
    private float dragGiven = 5f;
    private PlayerStatus status;

    void Start () 
    {
        Destroy(gameObject, 15f);
	}

    void OnDestroy()
    {
        status.insidePlague = false;
    }

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        status = player.GetComponent<PlayerStatus>();
        status.insidePlague = true;
        if (player.rigidbody2D.drag == 0)
        {
            player.rigidbody2D.drag += 15;
        }
    }

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
        timer += Time.deltaTime;
        status.insidePlague = true;
        if (timer > timeTilDrag)
        {
            player.rigidbody2D.drag += 10;
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
        status.insidePlague = false;
    }
}

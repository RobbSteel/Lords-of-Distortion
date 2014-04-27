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
        //particleSystem.renderer.sortingLayerName = "Foreground";
        Destroy(gameObject, 15f);
	}

    void OnDestroy()
    {
        if(status != null)
        { 
            status.RemovePlague();
        }
    }

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        if(status == null)
        {
            status = controller.GetComponent<PlayerStatus>();
        }

        if (status.PlagueAmount() == 0)
        {
            status.AddPlague(25f);
        }
    }

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
        if (status == null)
        {
            status = controller.GetComponent<PlayerStatus>();
        }

        timer += Time.deltaTime;
    
        if (timer > timeTilDrag)
        {
            status.AddPlague(12f);
            timer = 0;
            
            if (status.PlagueAmount() > 50)
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
        status.RemovePlague();
    }
}

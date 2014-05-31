using UnityEngine;
using System.Collections;

public class PlagueScript : Power 
{

	public GameObject targetplayer;
	public GameObject[] possibletargets;
	public float plaguespeed;
	public float closestdistance = 100;


    void Start () 
    {
        //particleSystem.renderer.sortingLayerName = "Foreground";
	    possibletargets = GameObject.FindGameObjectsWithTag("Player");
        Destroy(gameObject, 8f);
		AcquireTarget();
		plaguespeed = 0.02f;
	}

    void OnDestroy()
    {
      
    }

	void Update(){

		if(targetplayer != null){
			if(targetplayer.GetComponent<Controller2D>().dead){
				AcquireTarget();
			}
			transform.position = Vector2.MoveTowards(transform.position, targetplayer.transform.position, plaguespeed);
		}
	}

	void AcquireTarget(){

		for(int i = 0; i < possibletargets.Length; i++){
			if(possibletargets[i] != null){
				var currplayer = possibletargets[i];
				var distance = Vector2.Distance(transform.position, currplayer.transform.position);
					if((distance < closestdistance) && !currplayer.GetComponent<Controller2D>().dead){
						targetplayer = currplayer;
						closestdistance = distance;
					}
			}
		}

		closestdistance = 100;
	}

    public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
   
		controller.Die(DeathType.PLAGUE);

    }

    public override void PowerActionStay(GameObject player, Controller2D controller)
    {
    
		controller.Die(DeathType.PLAGUE);
    }

    public override void PowerActionExit(GameObject player, Controller2D controller)
    {
     
    }
}

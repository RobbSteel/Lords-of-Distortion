using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Priority_Queue;

public class PlagueScript : Power 
{
	public bool targetacquired = false;
	public GameObject targetplayer;
	public int targetplayernumber;
	public GameObject[] possibletargets;
	public float plaguespeed;
	public float closestdistance = 100;

	//Host acquires an initial target and relays which player it will be, just in case other players have
	// not populated their list of targets
    void Start () 
	{
	    possibletargets = GameObject.FindGameObjectsWithTag("Player");
		Destroy(gameObject, 8f);
		AcquireTarget();
		plaguespeed = 0.7f;
	}


	//Clients search for the host selected player, both check for dead players and move the plague
	void Update(){
	
		//If plague kills someone reacquire new targets
		if(targetplayer != null){
			if(targetplayer.GetComponent<Controller2D>().dead){
				targetacquired = false;
				AcquireTarget();
			}
			transform.position = Vector2.MoveTowards(transform.position, targetplayer.transform.position, plaguespeed * Time.deltaTime);
		}
	}

	void AcquireTarget(){
		if(Network.isServer){
		if(!targetacquired){
		//Look through alive players and target one
		for(int i = 0; i < possibletargets.Length; i++){
			if(possibletargets[i] != null){
				var currplayer = possibletargets[i];
				var distance = Vector2.Distance(transform.position, currplayer.transform.position);
					if((distance < closestdistance) && !currplayer.GetComponent<Controller2D>().dead){
						targetplayer = currplayer;
						targetplayernumber = i;
						closestdistance = distance;
					}
			}
		}

		//If no players are alive, reselect a dead player
		if(targetplayer == null){

			for(int i = 0; i < possibletargets.Length; i++){
				if(possibletargets[i] != null){
					var currplayer = possibletargets[i];
					var distance = Vector2.Distance(transform.position, currplayer.transform.position);
						if((distance < closestdistance)){
							targetplayernumber = i;
							targetplayer = currplayer;
							closestdistance = distance;
						}
				}
			}
		}
		
		var netview = targetplayer.networkView;
		var finaltarget = netview.owner;
		networkView.RPC("RelayTarget", RPCMode.Others, finaltarget);
		targetacquired = true;

			}
		}
		closestdistance = 100;
		
	}


	//Relay the target player number
	[RPC]
	void RelayTarget(NetworkPlayer playertarget){
	
		targetplayer = PlayerServerInfo.instance.GetPlayerGameObject(playertarget);
		print (targetplayer);
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

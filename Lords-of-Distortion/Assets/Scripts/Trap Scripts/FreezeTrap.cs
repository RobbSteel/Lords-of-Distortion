using UnityEngine;
using System.Collections;

public class FreezeTrap : Power {

	private PlayerStatus applyDmg;
	private float trapDuration = 0f;
	private GameObject frozenEffect;
	private Controller2D frozenplayer;
	private GameObject currentplayer;
	private bool used = false;
    private bool freezeExplosionActivated = false;

	public GameObject frozenEffectPrefab;

	void Start()
    {
        GetComponent<ParticleSystem>().loop = false;
		Destroy(gameObject, 3);
	}

	void Update()
    {
        trapDuration += Time.deltaTime;
        if(trapDuration > 2.2)
        {
            if(!freezeExplosionActivated)
            {
                GetComponent<CircleCollider2D>().radius = 1.25f;
                GameObject freezeExplosion = (GameObject)Instantiate(Resources.Load("FreezeExplode"));
                freezeExplosion.transform.position = transform.position;
                freezeExplosionActivated = true;
            }
            if(freezeExplosionActivated)
            {
                if(trapDuration > 2.5)
                {
                    GetComponent<CircleCollider2D>().radius = 0f;
                }
            }
        }
	}

	[RPC]
	void FreezeFollow(NetworkPlayer player){

		GameObject playerObject = SessionManager.Instance.psInfo.GetPlayerGameObject (player);
		var tempfreeze = (GameObject)Instantiate(frozenEffectPrefab ,playerObject.transform.position, Quaternion.identity);
		var playercontroller = playerObject.GetComponent<Controller2D>();
		applyDmg = playercontroller.GetComponent<PlayerStatus> ();
		applyDmg.Frozen();
		tempfreeze.GetComponent<FrozenEffect>().checkStun = playercontroller;
		print (tempfreeze.GetComponent<FrozenEffect>().checkStun);
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        applyDmg = controller.GetComponent<PlayerStatus>();
        if (trapDuration > 2.4)
        {
            if (!used)
            {
                used = true;

                player.rigidbody2D.drag = 0;
                applyDmg.Frozen();
                frozenEffect = (GameObject)Instantiate(Resources.Load("FrozenEffect"), player.transform.position, Quaternion.identity);
                frozenEffect.GetComponent<FrozenEffect>().checkStun = controller;
                frozenplayer = controller;
                currentplayer = player;

                networkView.RPC("FreezeFollow", RPCMode.Others, Network.player);
            }
        }
        else
        { 
            player.rigidbody2D.drag = 45;
        }

		if(GameObject.Find("CollectData") != null){
			GA.API.Design.NewEvent("Times Frozen", player.transform.position);
		}
		
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller)
    {
        applyDmg = controller.GetComponent<PlayerStatus>();
        if (trapDuration > 2.4)
        {
            if(!used)
            {
                used = true; 

                applyDmg.Frozen();
                player.rigidbody2D.drag = 0;
                frozenEffect = (GameObject)Instantiate(Resources.Load("FrozenEffect"), player.transform.position, Quaternion.identity);
                frozenEffect.GetComponent<FrozenEffect>().checkStun = controller;
                frozenplayer = controller;
                currentplayer = player;

				if(OFFLINE)
                networkView.RPC("FreezeFollow", RPCMode.Others, Network.player);
            }
        }
        else
        {
            player.rigidbody2D.drag = 45;
        }
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.drag = 0;
	}
}

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
	const float slowDuration = .5f;
	void Start()
    {
        GetComponent<ParticleSystem>().loop = false;
		Destroy(gameObject, 3f);
	}

    void OnDestroy()
    {
        currentplayer.rigidbody2D.drag = 0;
    }

	void Update()
    {
        trapDuration += Time.deltaTime;
		if(trapDuration >= slowDuration)
        {
            if(!freezeExplosionActivated)
            {
                GetComponent<CircleCollider2D>().radius = 1.8f;
                GameObject freezeExplosion = (GameObject)Instantiate(Resources.Load("FreezeExplode"));
                freezeExplosion.transform.position = transform.position;
                freezeExplosionActivated = true;
            }
            if(freezeExplosionActivated)
            {
                if(trapDuration > slowDuration + .5f)
                {
					GetComponent<CircleCollider2D>().enabled = false;
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
		tempfreeze.GetComponent<FrozenEffect>().player = playercontroller;
		print (tempfreeze.GetComponent<FrozenEffect>().player);
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller)
    {
        applyDmg = controller.GetComponent<PlayerStatus>();
		if (trapDuration > slowDuration)
        {
            if (!used)
            {
                used = true;

				player.rigidbody2D.drag = 0;
				applyDmg.Frozen();
                frozenEffect = (GameObject)Instantiate(Resources.Load("FrozenEffect"), player.transform.position, Quaternion.identity);
                frozenEffect.GetComponent<FrozenEffect>().player = controller;
                frozenplayer = controller;
                currentplayer = player;

                networkView.RPC("FreezeFollow", RPCMode.Others, Network.player);
				if(Analytics.Enabled){
					GA.API.Design.NewEvent("Times Frozen", player.transform.position);
				}
            }
        }
        else
        { 
            player.rigidbody2D.drag = 100f;
        }

	
		
	}
	
	public override void PowerActionStay(GameObject player, Controller2D controller)
    {
        applyDmg = controller.GetComponent<PlayerStatus>();
		if (trapDuration > slowDuration)
        {
            if(!used)
            {
                used = true; 

                applyDmg.Frozen();
                player.rigidbody2D.drag = 0;
                frozenEffect = (GameObject)Instantiate(Resources.Load("FrozenEffect"), player.transform.position, Quaternion.identity);
                frozenEffect.GetComponent<FrozenEffect>().player = controller;
                frozenplayer = controller;
                currentplayer = player;

				if(!OFFLINE)
                networkView.RPC("FreezeFollow", RPCMode.Others, Network.player);
            }
        }
        else
        {
            player.rigidbody2D.drag = 100f;
        }
	}
	
	public override void PowerActionExit(GameObject player, Controller2D controller)
    {
        player.rigidbody2D.drag = 0;
	}
}

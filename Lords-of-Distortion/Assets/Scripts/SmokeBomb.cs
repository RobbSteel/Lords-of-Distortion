using UnityEngine;
using System.Collections;

public class SmokeBomb : Power {
	
	public float timeTillDmg;
	public float dmgGiven;
	public float dmgTimer;
	private PlayerStatus applyDmg;
	public bool triggered;

	ParticleSystem smoke; 
	
	void Awake(){
		//triggered = false;
		smoke = GetComponent<ParticleSystem>();
		smoke.enableEmission = true;
	}
	
	// Use this for initialization
	void Start () 
    {
        Destroy(gameObject, 15f);
		//smoke.enableEmission = false;
	}
	
	// Update is called once per frame
	void Update () {
			//if(triggered)
				//Destroy(gameObject, 6f);

	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){
		//triggered = true;
		smoke.enableEmission = true;

		// calling on Trap Trigger function to destory UIpower when triggered
		//callOnTrapTrigger();

	}

	public override void PowerActionStay(GameObject player, Controller2D controller){
			dmgTimer += Time.deltaTime;
			if( dmgTimer >= timeTillDmg ){
				applyDmg = player.GetComponent<PlayerStatus>();
				applyDmg.TakeDamage( dmgGiven );
				dmgTimer = 0;
			}
	}

	public override void PowerActionExit(GameObject player, Controller2D controller){
		dmgTimer = timeTillDmg;
	}

}

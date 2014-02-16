using UnityEngine;
using System.Collections;

public class SmokeBomb : Power {
	
	public float timeTillDmg;
	public float dmgGiven;
	
	public float dmgTimer;
	private StunBar applyDmg;

	void Awake(){
		dmgTimer = timeTillDmg;
	}
	
	// Use this for initialization
	void Start () 
    {
        Destroy(gameObject, 6f);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void PowerActionEnter(GameObject player, Controller2D controller){

	}

	public override void PowerActionStay(GameObject player, Controller2D controller){
			dmgTimer += Time.deltaTime;
			if( dmgTimer >= timeTillDmg ){
				applyDmg = player.GetComponent<StunBar>();
				applyDmg.TakeDamage( dmgGiven );
				dmgTimer = 0;
			}
	}

	public override void PowerActionExit(GameObject player, Controller2D controller){
		dmgTimer = timeTillDmg;
	}

}

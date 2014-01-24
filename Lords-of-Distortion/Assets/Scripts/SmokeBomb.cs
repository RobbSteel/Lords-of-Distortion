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
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void PowerAction(GameObject player, Controller2D controller){
			dmgTimer += Time.deltaTime;
			if( dmgTimer >= timeTillDmg ){
				applyDmg = player.GetComponent<StunBar>();
				applyDmg.TakeDamage( dmgGiven );
				dmgTimer = 0;
			}
	}

	public override void OnLoseContact(GameObject player, Controller2D controller){
		dmgTimer = timeTillDmg;
	}

}

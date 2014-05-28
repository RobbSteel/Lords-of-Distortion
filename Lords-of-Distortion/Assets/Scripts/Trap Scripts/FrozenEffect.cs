using UnityEngine;
using System.Collections;

public class FrozenEffect : MonoBehaviour {

	public Controller2D player;
	private bool setupEffect;


	// Use this for initialization
	void Start () {
		setupEffect = false;
		this.particleSystem.renderer.sortingLayerName = "Player";
		this.particleSystem.renderer.sortingOrder = 100;
	}

	// Update is called once per frame
	void Update () {
		if(player != null){
			transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 10);
		}
		DestroyEffect();
	}

	void DestroyEffect(){
		if (player != null) {
			Setup();
			if (!player.stunned || player.dead)
            {
				Destroy(this.gameObject);
			}
		} 
        else
        {
            Destroy(this.gameObject);
        }
	}

	void Setup(){
		if (!setupEffect) {
			transform.position = player.transform.position;
			this.transform.parent = player.transform;
			setupEffect = true;
		}
	}


}

using UnityEngine;
using System.Collections;

public class FrozenEffect : MonoBehaviour {

	public Controller2D checkStun;
	private bool setupEffect;


	// Use this for initialization
	void Start () {
		setupEffect = false;
		this.particleSystem.renderer.sortingLayerName = "Player";
		this.particleSystem.renderer.sortingOrder = 100;
	}

	// Update is called once per frame
	void Update () {
		if(checkStun != null){
			transform.position = Vector2.MoveTowards(transform.position, checkStun.transform.position, 10);
		}
		DestroyEffect();
	}

	void DestroyEffect(){
		if (checkStun != null) {
			Setup();
			if (!checkStun.stunned) {
				Destroy(this.gameObject);
			}
		} 
	}

	void Setup(){
		if (!setupEffect) {
			transform.position = checkStun.transform.position;
			this.transform.parent = checkStun.transform;
			setupEffect = true;
		}
	}


}

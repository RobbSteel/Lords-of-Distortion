using UnityEngine;
using System.Collections;

public class DeadLord : MonoBehaviour {


	private bool once = false;
	public bool isDead = false;

	// Use this for initialization
	void Start () {
	
		UIEventListener.Get(GameObject.Find("DeadFireball")).onClick += SpawnDeadPowers;
		UIEventListener.Get(GameObject.Find("DeadGravity")).onClick += SpawnDeadPowers;
		UIEventListener.Get(GameObject.Find("DeadSmoke")).onClick += SpawnDeadPowers;
		UIEventListener.Get(GameObject.Find("DeadSticky")).onClick += SpawnDeadPowers;

		isDead = false;
	}
	
	// Update is called once per frame
	void Update () {
		//print (isDead);
		if(isDead){

			if(!once){
				print("Back to Lord Screen");
				GetComponent<TweenPosition>().enabled = true;
				once = true;
			} else {




			}
		}
	}




	void SpawnDeadPowers(GameObject button){




	}




}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DeadLord : MonoBehaviour {


	private bool once = false;
	public bool isDead = false;
	private bool powerset = false;
	private GameObject currpower;
	public GameObject[] poweroptions;
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


				if(Input.GetMouseButtonDown(0) && powerset){
				var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
				mousePos.z = 0;
				Network.Instantiate(currpower, mousePos, transform.rotation, 0);
				powerset = false;
				}

			}
		}
	}




	void SpawnDeadPowers(GameObject button){


		UILabel powerlabel = button.GetComponentInChildren<UILabel>();
		var powertype = powerlabel.text;

		if(powertype == "Fireball"){
			currpower = (GameObject)poweroptions[0];
		}
		
		if(powertype == "Sticky Trap"){
			currpower = (GameObject)poweroptions[1];
		}
		
		if(powertype == "Gravity"){
			currpower = (GameObject)poweroptions[2];
		}
		
		if(powertype == "Smoke Bomb"){
			currpower = (GameObject)poweroptions[3];
			
		}

		powerset = true;
	}




}
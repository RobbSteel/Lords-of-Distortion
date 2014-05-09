using UnityEngine;
using System.Collections;

public class AddStage : MonoBehaviour {


	public string stagename = "bodied";
	public GameObject stagedisplay;
	PickManager pickselect;
	private float waittimer = 0;
	public bool pickonce = false;
	// Use this for initialization
	void Start () {
	
		pickselect = GameObject.Find("PickManager").GetComponent<PickManager>();
	}

	void Update(){

		if(waittimer <= 0){

			pickonce = false;
		
		} else {

			waittimer -= Time.deltaTime;
		}

	}

	[RPC]
	void FillBoxes(int boxnum){

		GameObject pickdisplay = (GameObject)Instantiate(stagedisplay, new Vector3(-8.5f +(boxnum * 1.5f),2.5f,0), transform.rotation);
		pickdisplay.transform.localScale *= .45f;
		var displayscript = pickdisplay.gameObject.GetComponent<StageSelect>();
		Destroy(displayscript);
		
	}

	void FillBox(int boxnum){

		GameObject pickdisplay = (GameObject)Instantiate(stagedisplay, new Vector3(-8.5f +(boxnum * 1.5f),2.5f,0), transform.rotation);
		pickdisplay.transform.localScale *= .45f;
		var displayscript = pickdisplay.gameObject.GetComponent<StageSelect>();
		Destroy(displayscript);


	}

	// Update is called once per frame
	void OnPress(){
		if(Network.isServer){
		if(stagename != "bodied"){
			for(int i = 0; i < pickselect.picks.Length; i++){
				if(pickselect.picks[i] == "empty" && !pickonce){
					pickselect.picks[i] = stagename;
					pickselect.numberofpicks++;
					waittimer = 0.3f;
					pickonce = true;
					FillBox(i + 1);
					networkView.RPC("FillBoxes", RPCMode.Others, i + 1);
					break;
				}
			}
			}
	}
	}
}

using UnityEngine;
using System.Collections;

public class PickManager : MonoBehaviour {

	public int numberofpicks = 0;
	SessionManager manager;
	public string[] picks = new string[4]{"empty","empty","empty","empty"};
	public GameObject pickbutton;
	public GameObject allUI;
	public bool moveonce = false;
	// Use this for initialization
	void OnNetworkLoadedLevel () {
		manager = SessionManager.Instance;

		if(Network.isServer){
		GameObject picklabel = (GameObject)Network.Instantiate(allUI, new Vector3(0, 0, 0), transform.rotation,0);
		
		}
	


	}



void ReposPick(){
		var pickbutton = GameObject.Find("PickStage(Clone)");
		pickbutton.transform.localScale = new Vector3(1, 1, 1);
		pickbutton.transform.localPosition = new Vector2(325, -250);

}

	bool sentLevelLoadRPC = false;
	// Update is called once per frame
	void Update () {
	

		if(numberofpicks == 4){

			for(int i = 0; i < 4; i++){

				manager.arenas[i] = picks[i];

			}
			if(!sentLevelLoadRPC){
			if(Network.isServer){
				manager.LoadNextLevel(false);
				sentLevelLoadRPC = true;
				
				}
		}
		}
	}
}

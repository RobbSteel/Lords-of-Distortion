using UnityEngine;
using System.Collections;

public class PickManager : MonoBehaviour {

	public int numberofpicks = 0;
	SessionManager manager;
    GameObject psinfo;
    PlayerServerInfo psinfoscript;
	public string[] picks;
    public UIGrid LevelGrid;
	public GameObject pickbutton;
	public GameObject allUI;
	public bool moveonce = false;
    bool sentLevelLoadRPC = false;
    int numberOfStages;
	//On load instantiate the picking UI
	void OnNetworkLoadedLevel () {
		manager = SessionManager.Instance;
        psinfo = GameObject.Find("PSInfo");
        if (psinfo != null)
        { 
            psinfoscript = psinfo.GetComponent<PlayerServerInfo>();
            numberOfStages = psinfoscript.numStages;
            picks = new string[numberOfStages];
            for(int i = 0; i < picks.Length; i++)
            {
                picks[i] = "empty";
            }
        }
		if(Network.isServer){
		    GameObject picklabel = (GameObject)Network.Instantiate(allUI, new Vector3(0, 0, 0), transform.rotation, 0);
		}
	}

	// Update is called once per frame
	void Update ()
    {
		//Check to see if the roster is filled
		if(numberofpicks == numberOfStages){

			for(int i = 0; i < numberOfStages; i++){

				manager.arenas[i] = picks[i];

			}

			//Play our set of levels
			if(!sentLevelLoadRPC){
				if(Network.isServer && manager != null){
					manager.LoadNextLevel(false);
					sentLevelLoadRPC = true;
				}
			}
		}
	}
}

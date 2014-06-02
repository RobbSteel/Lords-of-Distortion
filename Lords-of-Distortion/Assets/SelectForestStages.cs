using UnityEngine;
using System.Collections;

public class SelectForestStages : MonoBehaviour {

    public GameObject MineSelection;
    public Transitioner Transition;
	public StageTracker trackmaster;
	public UIPanel SelectUI;
	public GameObject levelgrid;

    //private GameObject SelectUI;

    void Awake()
    {
        //SelectUI = GameObject.Find("SelectUI(Clone)");
    }

	[RPC]
	void Flip(){

		MineSelection.SetActive(false);
		levelgrid.SetActive(false);
		SelectUI.alpha = 0;
		trackmaster.Destroy();
		Transition.Flip("None-Forest", true);
		gameObject.SetActive(false);

	}


    void OnClick()
    {
		if(Network.isServer){
		MineSelection.SetActive(false);
		levelgrid.SetActive(false);
		SelectUI.alpha = 0;
		trackmaster.Destroy();
        Transition.Flip("None-Forest", true);
		networkView.RPC("Flip", RPCMode.Others);
        gameObject.SetActive(false);
		}
    }
}

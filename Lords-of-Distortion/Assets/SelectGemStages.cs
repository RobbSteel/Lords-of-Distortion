using UnityEngine;
using System.Collections;

public class SelectGemStages : MonoBehaviour {

    public GameObject ForestSelection;
    public Transitioner Transition;
	public StageTracker trackmaster;
	public UIPanel SelectUI;
	public GameObject levelgrid;
    //private GameObject SelectUI;

	[RPC]
	void Flip(){
		
		ForestSelection.SetActive(false);
		levelgrid.SetActive(false);
		SelectUI.alpha = 0;
		trackmaster.Destroy();
		Transition.Flip("None-Gem", false);
		gameObject.SetActive(false);
		
	}


    void OnClick()
    {
        if(Network.isServer){
		ForestSelection.SetActive(false);
		levelgrid.SetActive(false);
		SelectUI.alpha = 0;
		trackmaster.Destroy();
        Transition.Flip("None-Gem", false);
		networkView.RPC("Flip", RPCMode.Others);
        gameObject.SetActive(false);
		}
        /*SelectUI = GameObject.Find("SelectUI(Clone)");
        if (SelectUI != null)
            SelectUI.SetActive(false);*/
    }
}

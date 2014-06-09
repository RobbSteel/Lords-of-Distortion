using UnityEngine;
using System.Collections;

public class SelectForestStages : MonoBehaviour {

    public GameObject PreviousSelection;
    public Transitioner Transition;
	public StageTracker trackmaster;
	public UIPanel SelectUI;
    public GameObject levelgrid; 
    public bool turnBackwards;
    public GameObject otherButton;
	public AudioClip pageturn;

	[RPC]
	void Flip(){

		PreviousSelection.SetActive(false);
		levelgrid.SetActive(false);
		SelectUI.alpha = 0;
		trackmaster.Destroy();
		Transition.Flip("None-Forest", turnBackwards);
        if (otherButton != null)
            otherButton.SetActive(false);
        gameObject.SetActive(false);
	}

    void OnClick()
    {
		audio.PlayOneShot(pageturn);


		if(Network.isServer){
		    PreviousSelection.SetActive(false);
		    levelgrid.SetActive(false);
		    SelectUI.alpha = 0;
		    trackmaster.Destroy();
            Transition.Flip("None-Forest", turnBackwards);
		    networkView.RPC("Flip", RPCMode.Others);
            if (otherButton != null)
                otherButton.SetActive(false);
            gameObject.SetActive(false);
		}
    }
	void OnPress(bool isDown){
		if(isDown) audio.PlayOneShot(pageturn);
	}
}

using UnityEngine;
using System.Collections;

public class SelectCageStages : MonoBehaviour
{
    public GameObject PreviousSelection;
    public Transitioner Transition;
    public StageTracker trackmaster;
    public UIPanel SelectUI;
    public GameObject levelgrid;
    public GameObject otherButton;
    public bool turnBackwards;

    [RPC]
    void Flip()
    {
        PreviousSelection.SetActive(false);
        levelgrid.SetActive(false);
        SelectUI.alpha = 0;
        trackmaster.Destroy();
        Transition.Flip("None-Cage", turnBackwards);
        if (otherButton != null)
            otherButton.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnClick()
    {
        if (Network.isServer)
        {
            PreviousSelection.SetActive(false);
            levelgrid.SetActive(false);
            SelectUI.alpha = 0;
            trackmaster.Destroy();
            Transition.Flip("None-Cage", turnBackwards);
            networkView.RPC("Flip", RPCMode.Others);
            if (otherButton != null)
                otherButton.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
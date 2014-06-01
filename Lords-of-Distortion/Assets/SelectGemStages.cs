using UnityEngine;
using System.Collections;

public class SelectGemStages : MonoBehaviour {

    public GameObject ForestSelection;
    public Transitioner Transition;
    //private GameObject SelectUI;

    void Awake()
    {
        //SelectUI = GameObject.Find("SelectUI(Clone)");
    }

    void OnClick()
    {
        ForestSelection.SetActive(false);
        Transition.Flip("None-Gem", false);
        gameObject.SetActive(false);
        /*SelectUI = GameObject.Find("SelectUI(Clone)");
        if (SelectUI != null)
            SelectUI.SetActive(false);*/
    }
}

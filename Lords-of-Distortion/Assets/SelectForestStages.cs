using UnityEngine;
using System.Collections;

public class SelectForestStages : MonoBehaviour {

    public GameObject MineSelection;
    public Transitioner Transition;
    //private GameObject SelectUI;

    void Awake()
    {
        //SelectUI = GameObject.Find("SelectUI(Clone)");
    }

    void OnClick()
    {
        MineSelection.SetActive(false);
        Transition.Flip("None-Forest", true);
        gameObject.SetActive(false);
        /*SelectUI = GameObject.Find("SelectUI(Clone)");
        if (SelectUI != null)
            SelectUI.SetActive(false);
         */
    }
}

﻿using UnityEngine;
using System.Collections;

public class SelectGemStages : MonoBehaviour {

    public GameObject ForestSelection;
    public Transitioner Transition;
	public StageTracker trackmaster;
	public UIPanel SelectUI;
	public GameObject levelgrid;
    //private GameObject SelectUI;

    void Awake()
    {
        //SelectUI = GameObject.Find("SelectUI(Clone)");
    }

    void OnClick()
    {
        ForestSelection.SetActive(false);
		levelgrid.SetActive(false);
		SelectUI.alpha = 0;
		trackmaster.Destroy();
        Transition.Flip("None-Gem", false);
        gameObject.SetActive(false);
        /*SelectUI = GameObject.Find("SelectUI(Clone)");
        if (SelectUI != null)
            SelectUI.SetActive(false);*/
    }
}

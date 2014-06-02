using UnityEngine;
using System.Collections;

public class StageTracker : MonoBehaviour {

	public GameObject stagedisplay;
	public GameObject stagedifficulty;
	public GameObject stagename;


public void Destroy(){

		if(stagedisplay != null){
			Destroy(stagedisplay);
		}

		if(stagedifficulty != null){
			Destroy(stagedifficulty);
		}

		if(stagename != null){
			Destroy(stagename);
		}
	}


}

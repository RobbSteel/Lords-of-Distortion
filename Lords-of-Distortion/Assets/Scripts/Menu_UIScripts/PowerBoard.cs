using UnityEngine;
using System.Collections;

public class PowerBoard : MonoBehaviour {

	public PowerSlot currentPower;
	public int index;
	
	public void SetChild(PowerSlot power){
		currentPower = power;
		//UIStretch stretch = currentPower.gameObject.AddComponent<UIStretch>();
		//stretch.container = this.gameObject;
		//stretch.style = UIStretch.Style.FitInternalKeepingRatio;
	}

	public void RemoveChild(){
		NGUITools.Destroy(currentPower.gameObject);
		currentPower = null;
	}

	public PowerType GetType(){
		return currentPower.associatedPower.type;
	}
}

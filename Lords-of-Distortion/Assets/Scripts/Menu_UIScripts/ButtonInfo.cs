using UnityEngine;
using System.Collections;

public class ButtonInfo : MonoBehaviour {

	public InventoryPower associatedPower;
	UILabel nameLabel;
	UILabel quantityLabel;
	bool infinitePowers;

	public void Initialize(InventoryPower power, bool infinity){
		associatedPower = power;
		nameLabel = transform.Find("PowerName").GetComponent<UILabel>();
		quantityLabel = transform.Find("PowerQuantity").GetComponent<UILabel>();
		nameLabel.text = associatedPower.name;
		infinitePowers = infinity;
		UpdateQuantity();
	}

	public void UpdateQuantity(){
		if(!infinitePowers){
			quantityLabel.text = associatedPower.quantity.ToString();
		}
		else
			quantityLabel.text = "∞";
	}

}

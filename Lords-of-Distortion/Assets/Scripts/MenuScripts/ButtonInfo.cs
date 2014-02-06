using UnityEngine;
using System.Collections;

public class ButtonInfo : MonoBehaviour {

	public InventoryPower associatedPower;
	UILabel nameLabel;
	UILabel quantityLabel;
	// Use this for initialization
	void Start () {
	
	}
	public void Initialize(InventoryPower power){
		associatedPower = power;
		nameLabel = transform.Find("PowerName").GetComponent<UILabel>();
		quantityLabel = transform.Find("PowerQuantity").GetComponent<UILabel>();
		nameLabel.text = associatedPower.name;
		quantityLabel.text = associatedPower.quantity.ToString();
	}

	public void UpdateQuantity(){
		quantityLabel.text = associatedPower.quantity.ToString();
	}

}

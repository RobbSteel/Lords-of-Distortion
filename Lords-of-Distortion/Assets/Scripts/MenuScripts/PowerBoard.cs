using UnityEngine;
using System.Collections;

public class PowerBoard : MonoBehaviour {

	public InventoryPower associatedPower;

	public void Initialize(InventoryPower power, Sprite sprite){
		associatedPower = power;
		GetComponent<UI2DSprite>().sprite2D = sprite;
	}
}

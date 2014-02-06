using UnityEngine;
using System.Collections;

public class InventoryPower {
	
	public PowerType type{get; set;}
	public int quantity {get; set;}
	public string name {get; set;}

	public InventoryPower(PowerType type, int quantity, string name = null){
		this.type = type;
		this.quantity = quantity;

		if(name == null)
			this.name = type.ToString();
		else
			this.name = name;
	}
}

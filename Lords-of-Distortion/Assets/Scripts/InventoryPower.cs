using UnityEngine;
using System.Collections;

public class InventoryPower {
	
	public PowerType type{get; set;}
	public int quantity {get; set;}
	public string name {get; set;}
	public bool infinite {get; set;}

	public InventoryPower(PowerType type, bool infinite, string name = null){
		this.type = type;
		this.infinite = infinite;
		quantity = 1;
		if(name == null)
			this.name = type.ToString();
		else
			this.name = name;
	}
}

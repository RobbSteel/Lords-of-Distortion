using UnityEngine;
using System.Collections;

public abstract class Power : MonoBehaviour {

	public abstract void PowerAction(GameObject player, Controller2D controller);
	public abstract void OnLoseContact(GameObject player, Controller2D controller);

}

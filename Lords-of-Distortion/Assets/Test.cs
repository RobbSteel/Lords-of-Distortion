using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	
	IEnumerator Do() {
		//Debug.Log("Do now");
		yield return null;
		//Debug.Log("Do 2 seconds later");

	}

	void Start() {
		Do();
		//Debug.Log("This is printed immediately");
	}
}

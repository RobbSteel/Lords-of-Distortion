using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	
	IEnumerator YieldSign() {
		print ("spawn yield symbol");
		yield return new WaitForSeconds(2);
		PowerSpawn();
	}

	void  PowerSpawn(){
		print ("spawn power");
	}

	void Start() {
		StartCoroutine(YieldSign());
		print ("disable button");
	}
}

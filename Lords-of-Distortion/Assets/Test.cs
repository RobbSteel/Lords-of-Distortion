using UnityEngine;
using System.Collections;
using System.Linq;
public class Test : MonoBehaviour {

	/*
	 * coroutine
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
*/

	void Start(){
		CircularBuffer<int> buffer = new CircularBuffer<int>(2);
		buffer.Add(1);
		buffer.Add(2);
		buffer.Add(3);
		foreach(int i in buffer.Reverse<int>()){
			print (i);
		}
	}
}

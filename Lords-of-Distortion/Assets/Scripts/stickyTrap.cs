using UnityEngine;
using System.Collections;

public class stickyTrap : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		stickyT();
	}
	
	public void stickyT()
	{
		
		if (Input.GetKeyDown(KeyCode.T) == true)
		{
			// Local Spawn
			GameObject stickyt = (GameObject)Instantiate(Resources.Load("stickyTrap"));
			Destroy(stickyt, 5f);
			
		}
	}
	
	void OnDestroy()
	{
		Debug.Log("Destroyed");
		GameObject user = GameObject.FindGameObjectWithTag("Player");
		if (user != null)
			user.rigidbody2D.drag = 0;
	}
	
}



using UnityEngine;
using System.Collections;

public class stickyTrap : Power {
	
	// Update is called once per frame
	void Update () {
		stickyT();
	}

	Controller2D affected;
	public override void PowerAction (GameObject player, Controller2D controller){
		affected = controller;
		player.rigidbody2D.drag = 40;
		affected.canJump = false;
		Debug.Log("Hit sticky trap");
	}

	public override void OnLoseContact (GameObject player, Controller2D controller){
		player.rigidbody2D.drag = 0;
		affected.canJump = true;
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
		if (user != null){
			user.rigidbody2D.drag = 0;
		}
		if(affected != null)
			affected.canJump = true;
	}
	
}


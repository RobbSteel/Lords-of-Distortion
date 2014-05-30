using UnityEngine;
using System.Collections;
using InControl;



public class GameInput : MonoBehaviour {


	public static GameInput instance;
	// Use this for initialization

	public bool usingGamePad = true;


	public KeyCode JumpKey = KeyCode.Space;

	void Awake()
	{
		if(instance != null){
			Destroy(this.gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
	}


	void Start () {
		InputManager.Setup();
	}

	void Update () {
		InputManager.Update();
	}
}



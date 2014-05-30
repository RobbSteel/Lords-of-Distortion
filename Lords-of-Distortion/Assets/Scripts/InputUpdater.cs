using UnityEngine;
using System.Collections;
using InControl;

public class InputUpdater : MonoBehaviour {


	public static InputUpdater instance;
	// Use this for initialization

	public bool usingGamePad = true;

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

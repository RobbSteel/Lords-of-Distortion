using UnityEngine;
using System.Collections;
using InControl;



public class GameInput : MonoBehaviour {


	public static GameInput instance;
	// Use this for initialization

	public bool usingGamePad = true;


	//Use this instead of Input.mousePosition
	public Vector2 cursorPosition;
	public Texture2D CursorTexture;
	float analogSensitity = 750f;

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
		UseCustomCursor(true); //TODO: call this in game instead of menu
		analogSensitity = Screen.width *.90f;
	}

	void Update () {
		InputManager.Update();
		GamePadToCursor();
	}

	bool useCustomCursor = false;

	public void UseCustomCursor(bool use)
	{
		Screen.showCursor = !use;
		useCustomCursor = use;
	}

	private Vector2 oldMousePosition;
	private Rect cursorRect = new Rect(0f, 0f, 32f, 32f);



	void GamePadToCursor()
	{
		InputDevice device = InputManager.ActiveDevice;
		cursorPosition.x += device.RightStickX.Value * analogSensitity * Time.deltaTime;
		cursorPosition.y += -device.RightStickY.Value * analogSensitity * Time.deltaTime;
	}

	void OnGUI()
	{
		Vector2 mousePosition = Event.current.mousePosition;
		//We moved our mouse. Ignore gamepad's cursor location and use mouse's
		if(oldMousePosition != mousePosition)
		{
			cursorPosition = mousePosition;
		}
		oldMousePosition = mousePosition;

		if(useCustomCursor)
		{
			cursorRect.position = new Vector2(cursorPosition.x - 16f, cursorPosition.y - 16f);
			GUI.DrawTexture (cursorRect, CursorTexture);
		}
	}
}



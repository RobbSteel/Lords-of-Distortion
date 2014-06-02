using UnityEngine;
using System.Collections;
using InControl;



public class GameInput : MonoBehaviour {


	public static GameInput instance;
	// Use this for initialization

	public bool usingGamePad = true;


	//Use this instead of Input.mousePosition
	private Vector2 cursorPosition;
	public Vector2 MousePosition;

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
		analogSensitity = Screen.width *.90f;
	}

	int inactiveFrames = 0;
	void Update () {

		InputManager.Update();
		InputDevice activeDevice = InputManager.ActiveDevice;
		//Were using the gamepad if its state has changed.
		if(activeDevice.AnyButton.IsPressed || activeDevice.Direction.State || activeDevice.RightStick.State
		                || activeDevice.RightTrigger.IsPressed || activeDevice.LeftTrigger.IsPressed || activeDevice.RightBumper.IsPressed
		   				|| activeDevice.LeftBumper.IsPressed)
		{
			inactiveFrames = 0;
			usingGamePad = true;
		}
		else 
		{
			inactiveFrames++;
		}
		if(inactiveFrames > 1)
			usingGamePad = false;


		if(usingGamePad && useCustomCursor)
			GamePadToCursor();
	}

	bool useCustomCursor = false;

	public void UseCustomCursor()
	{
		Screen.showCursor = false;
		useCustomCursor = true;
	}

	public void HideCustomCursor(){
		useCustomCursor = false;
	}

	private Vector2 oldMousePosition;
	private Rect cursorRect = new Rect(0f, 0f, 32f, 32f);



	void GamePadToCursor()
	{
		InputDevice device = InputManager.ActiveDevice;
		cursorPosition.x += device.RightStickX.Value * analogSensitity * Time.deltaTime;
		cursorPosition.y += -device.RightStickY.Value * analogSensitity * Time.deltaTime;

		MousePosition.x = cursorPosition.x;
		MousePosition.y = Camera.main.pixelHeight -  cursorPosition.y; //flip coordinates
	}
	
	void OnGUI()
	{
		Vector2 mousePositionUI = Event.current.mousePosition;
		//We moved our mouse. Ignore gamepad's cursor location and use mouse's
		if(!usingGamePad && oldMousePosition != mousePositionUI)
		{
			cursorPosition = mousePositionUI;
			MousePosition.x = cursorPosition.x;
			MousePosition.y = Camera.main.pixelHeight -  cursorPosition.y;
		}

		oldMousePosition = mousePositionUI;

		if(useCustomCursor)
		{
			cursorRect.center = new Vector2(cursorPosition.x, cursorPosition.y);
			GUI.DrawTexture (cursorRect, CursorTexture);
		}
	}
}



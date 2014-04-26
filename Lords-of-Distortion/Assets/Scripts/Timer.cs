using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
	
	public UILabel label;
	
	public float countDownTime;
	public float preGameTimer;
	public float postmatchtimer;
	public float lastmantimer;
	public float matchTime = 30f;
	public Vector3 centerscreen;
	private bool once = false;
	public int CurrentTimer;
	public ArenaManager arenaManager;
	private Vector3 defaultTimerPosition;

	void Awake(){
		label = GetComponent<UILabel>();
	}

	void Start(){
		CurrentTimer = 0;
		defaultTimerPosition = this.transform.position;
		Vector3 screenPos = GameObject.Find("Camera").gameObject.transform.position;
		//float screenHeight = Screen.height;
		//float screenWidth = Screen.width;
		//screenPos.x -= (screenWidth / 2.0f);
		//screenPos.y -= (screenHeight / 2.0f);
		centerscreen = screenPos;
		Debug.Log( "Center Pos:" + centerscreen );
		this.transform.position = centerscreen;
		Debug.Log( "Default Pos:" + defaultTimerPosition );
		
	}
	
	void Update()
	{
		countDownTime -= Time.deltaTime;
		TimerUI();
	}

	public void Show(){
		this.transform.localPosition = new Vector2(0, 350);
	}

	public void Hide(){

		this.transform.localPosition = Vector2.zero;
	}

	void TimerUI(){
		int minutes = Mathf.FloorToInt(countDownTime / 60F);
		int seconds = Mathf.FloorToInt(countDownTime - minutes * 60);
		string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
		
		label.text = niceTime;
	}
}

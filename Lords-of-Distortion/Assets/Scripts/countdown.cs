using UnityEngine;
using System.Collections;

public class countdown : MonoBehaviour {
	
	public UILabel myLabel;
	
	public float myTimer;
	public float powerPlaceTimer;
	public float fightCountdown;
	public float postmatchtimer;
	public float matchTime = 30f;
	private bool once = false;
	public int CurrentTimer;
	public ArenaManager arenaManager;
	private Vector3 defaultTimerPosition;
	// Update is called once per frame

	SessionManager sessionManager;

	void Awake(){
		
		sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
	}

	void Start(){
		myTimer = powerPlaceTimer;
		CurrentTimer = 0;
		defaultTimerPosition = this.transform.position;
		Vector3 screenPos = GameObject.Find("Camera").gameObject.transform.position;
		//float screenHeight = Screen.height;
		//float screenWidth = Screen.width;
		//screenPos.x -= (screenWidth / 2.0f);
		//screenPos.y -= (screenHeight / 2.0f);
		Vector3 centerScreenPosition = screenPos;
		Debug.Log( "Center Pos:" + centerScreenPosition );
		this.transform.position = centerScreenPosition;
		Debug.Log( "Default Pos:" + defaultTimerPosition );
		
	}
	
	void Update()
	{
		TimerUI();
		PlacementTimer();
		FightCountDownTimer();
		MatchStartTimer();
		MatchEndTimer();
		
	}
	
	void PlacementTimer(){
		if( CurrentTimer == 0 ){
			myTimer -= Time.deltaTime;
			
			if( myTimer <= 0 ){
				resetTimer( fightCountdown + 1 );
			}
		}
	}
	
	void FightCountDownTimer(){
		if( CurrentTimer == 1 ){
			myTimer -= Time.deltaTime;
			
			if( myTimer <= 0 ){

				resetTimer( 1 );

				this.transform.localPosition = defaultTimerPosition;
			}
		}
	}
	
	void MatchStartTimer(){
		if( CurrentTimer == 2 ){
			myTimer += Time.deltaTime;

			if(arenaManager.finishgame == true){
				print ("got here hello");
				resetTimer (5);
			}

		}
	}

	//Counts down 5 seconds after final player death and then loads the next level.
	void MatchEndTimer(){
		
		if(CurrentTimer == 3){
			myTimer -= Time.deltaTime;

			if(myTimer <= 0){

				if(Network.isServer && !once){
					once = true;
					sessionManager.LoadNextLevel(true);
				}
			}
			
		}
		
	}





	void TimerUI(){
		int minutes = Mathf.FloorToInt(myTimer / 60F);
		int seconds = Mathf.FloorToInt(myTimer - minutes * 60);
		string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
		
		myLabel.text = niceTime;
	}
	
	void resetTimer( float startTime ){
		myTimer = startTime;
		CurrentTimer++;
	}

	public float GetTimer()
	{
		return myTimer;
	}
	
	
	
}

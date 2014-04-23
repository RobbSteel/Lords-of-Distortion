using UnityEngine;
using System.Collections;

public class countdown : MonoBehaviour {
	
	public UILabel myLabel;
	
	public float myTimer;
	public float preGameTimer;
	public float postmatchtimer;
	public float lastmantimer;
	public float matchTime = 30f;
	public Vector3 centerscreen;
	private bool once = false;
	public int CurrentTimer;
	public ArenaManager arenaManager;
	private Vector3 defaultTimerPosition;
	// Update is called once per frame

	SessionManager sessionManager;

	void Awake(){
		
		sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
		arenaManager = GameObject.FindWithTag("ArenaManager").GetComponent<ArenaManager>();

	}

	void Start(){
		myTimer = preGameTimer;
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
		TimerUI();
		PreGameTimer();
		MatchStartTimer();
		KillLastPlayer();
		MatchEndTimer();
		
	}
	
	void PreGameTimer(){
		if( CurrentTimer == 0 ){
			myTimer -= Time.deltaTime;
			
			if( myTimer <= 0 ){
				resetTimer( 1 );
				this.transform.localPosition = defaultTimerPosition;
			}
		}
	}

	void MatchStartTimer(){
		if( CurrentTimer == 1 ){
			myTimer += Time.deltaTime;

			if(arenaManager.finishgame == true){
				print ("got here hello");
				this.transform.localPosition = new Vector2(0, 350);
				resetTimer (postmatchtimer);
			}

			if(arenaManager.lastman == true){
				this.transform.localPosition = new Vector2(0, 350);
				resetTimer (lastmantimer);
			}

		}
	}

	void KillLastPlayer(){

		if(CurrentTimer == 2){

			myTimer -= Time.deltaTime;
			 
			if(arenaManager.finishgame == true){
				print ("last player died");
				this.transform.localPosition = new Vector2(0, 350);
				resetTimer(postmatchtimer);
			}

			if(myTimer <= 0){
			
					arenaManager.finishgame = true;
				arenaManager.lastmanvictory = true;
					print ("player won");
					resetTimer (postmatchtimer);

				
			}
		}
	}


	//Counts down 5 seconds after final player death and then loads the next level.
	void MatchEndTimer(){

		if(CurrentTimer == 3){
			myTimer -= Time.deltaTime;

			if(myTimer <= .2){
				if(GameObject.FindGameObjectWithTag("Player") != null){
					var tempplayer = GameObject.FindGameObjectWithTag("Player");
					var tempscript = tempplayer.GetComponent<Controller2D>();
					tempscript.Die();
					print ("MERCY");
				}

			}

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

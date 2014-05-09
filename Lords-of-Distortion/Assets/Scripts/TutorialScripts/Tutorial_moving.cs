using UnityEngine;
using System.Collections;

public class Tutorial_moving: MonoBehaviour {
	
	public Camera mainCam;
	public GameObject player;
	private string nextLevel = "Tutorial_Menu";
	public UILabel guiText;
	public GameObject[] objectives;
	public int currentObjective;
	public string firstMessage;
	public string secondMessage;
	public string deathMessage;
	public string finishedMessage;
	private SceneFadeInOut transitionToNewScene;
	private bool endScene;
	private float fadeTimer;
	private float durationOfFading;
	private bool fade;

	void Awake(){
		
	}
	
	// Use this for initialization
	void Start () {
		fade = false;
		durationOfFading = 1;
		fadeTimer = 0;
		transitionToNewScene = this.GetComponent<SceneFadeInOut> ();
		currentObjective = 0;
		objectives [currentObjective].SetActive (true);
		guiText = GameObject.Find ("HUDText").GetComponent<UILabel>();
		changeText( firstMessage );
		runScene(currentObjective);
		endScene = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		textAnimation ();
		
		if (endScene)
			loadNextLevel ();
		
		if (player == null)
			resetLevel ();
		
		incrementObjective();
	}
	
	private void textAnimation(){
		if (fade)
			fadeTextClear ();
		
		if (!fade)
			fadeTextSolid ();
	}
	
	private void setFadeClear(){
		fadeTimer = 0;
		fade = true;
	}
	
	private void setFadeSolid(){
		fadeTimer = 0;
		fade = false;
	}
	
	private void incrementObjective(){
		if ( !endScene && objectives [currentObjective] == null ) {
			currentObjective += 1;
			
			if( currentObjective < objectives.Length ){
				runScene(currentObjective);
				objectives[currentObjective].SetActive(true);
			}
			else if (currentObjective >= objectives.Length ){
				endScene = true;
			}
		}
	}
	
	private void changeText( string newText ){
		guiText.text = newText;
	}
	
	private void runScene( int current ){
		switch( current ){
		case 0:
			StartCoroutine( startScene());
			break;
		default:
			break;
		}
	}
	
	private void fadeTextClear(){
		fadeTimer += Time.deltaTime;
		Color current = Color.Lerp (Color.white, Color.clear, fadeTimer/durationOfFading );
		guiText.color = current;
	}
	
	private void fadeTextSolid(){
		fadeTimer += Time.deltaTime;
		Color current = Color.Lerp (Color.clear, Color.white, fadeTimer/durationOfFading );
		guiText.color = current;
	}
	
	IEnumerator startScene(){
		//player.GetComponent<Controller2D> ().locked = true;
		yield return new WaitForSeconds (1);
		setFadeClear ();
		yield return new WaitForSeconds (durationOfFading);
		setFadeSolid ();
		changeText (secondMessage);
		//player.GetComponent<Controller2D> ().locked = false;
		
	}
	
	public void loadNextLevel(){
		changeText (finishedMessage);
		transitionToNewScene.EndScene( nextLevel );
	}
	
	public void resetLevel(){
		changeText (deathMessage);
		Application.LoadLevel (Application.loadedLevel);
	}
	
	
}

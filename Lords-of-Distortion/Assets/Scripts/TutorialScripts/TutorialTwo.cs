using UnityEngine;
using System.Collections;

public class TutorialTwo: MonoBehaviour {
	
	public Camera mainCam;
	public GameObject player;
	private string nextLevel = "Tutorial-Three";
	public UILabel guiText;
	public GameObject[] objectives;
	public int currentObjective;
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
		durationOfFading = 2;
		fadeTimer = 0;
		transitionToNewScene = this.GetComponent<SceneFadeInOut> ();
		currentObjective = 0;
		objectives [currentObjective].SetActive (true);
		guiText = GameObject.Find ("HUDText").GetComponent<UILabel>();
		changeText( "How to use your Melee" );
		runScene(currentObjective);
		endScene = false;
	}
	
	// Update is called once per frame
	void Update () {

		textAnimation ();

		if (endScene)
			loadNextLevel ();

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
		if (objectives [currentObjective] == null) {
			currentObjective += 1;
			
			if( currentObjective < objectives.Length ){
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
		player.GetComponent<Controller2D> ().locked = true;
		yield return new WaitForSeconds (2);
		setFadeClear ();
		yield return new WaitForSeconds (durationOfFading);
		setFadeSolid ();
		changeText ("Melee Dummy into Spikes");
		player.GetComponent<Controller2D> ().locked = false;
		
	}
	
	public void loadNextLevel(){
		changeText ("FINISHED");
		transitionToNewScene.EndScene( nextLevel );
	}
	
	
}

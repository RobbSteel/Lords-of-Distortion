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
	
	void Awake(){

	}
	
	// Use this for initialization
	void Start () {
		transitionToNewScene = this.GetComponent<SceneFadeInOut> ();
		currentObjective = 0;
		objectives [currentObjective].SetActive (true);
		guiText = GameObject.Find ("HUDText").GetComponent<UILabel>();
		changeText( "Welcome to Paper Project" );
		runScene(currentObjective);
		endScene = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (endScene)
			loadNextLevel ();

		incrementObjective();
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
	
	IEnumerator startScene(){
		player.GetComponent<Controller2D> ().locked = true;
		yield return new WaitForSeconds (2);
		changeText ("Kill the Dummy");
		player.GetComponent<Controller2D> ().locked = false;
		
	}
	
	public void loadNextLevel(){
		changeText ("FINISHED");
		transitionToNewScene.EndScene( nextLevel );
	}
	
	
}

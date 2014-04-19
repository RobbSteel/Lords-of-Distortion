using UnityEngine;
using System.Collections;

public class TutorialOne : MonoBehaviour {
		
		public Camera mainCam;
		public GameObject player;
		private int currentLevel;
		private string nextLevel = "Tutorial-Two";
		private SceneFadeInOut transitionToNewScene;
		public UILabel guiText;
		public GameObject[] objectives;
		public int currentObjective;
		private bool endScene;
		private float resetTimer;
		private float timer;
		
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
				transitionToNewScene.EndScene( nextLevel );

			if (player = null)
				resetLevel ();

			incrementObjective();
		}
		
		private void incrementObjective(){
		if ( currentObjective < objectives.Length && objectives[currentObjective] == null ) {
				currentObjective += 1;
				
				if( currentObjective < objectives.Length ){
					objectives[currentObjective].SetActive(true);
				}
				if( currentObjective >= objectives.Length ){
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
			changeText ("Get To the way points");
			player.GetComponent<Controller2D> ().locked = false;

		}
		
		public void loadNextLevel(){
			changeText ("FINISHED");
			transitionToNewScene.EndScene( nextLevel );
		}

		public void resetLevel(){
			changeText ("You Died");
			Application.LoadLevel (Application.loadedLevel);
		}

}

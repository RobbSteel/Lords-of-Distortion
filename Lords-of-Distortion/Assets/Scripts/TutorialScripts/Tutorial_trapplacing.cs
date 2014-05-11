using UnityEngine;
using System.Collections;

public class Tutorial_trapplacing: MonoBehaviour {
	
	public Camera mainCam;
	public GameObject player;
	private string nextLevel = "Tutorial_Menu";
	public UILabel guiText;
	public GameObject[] objectives;
	public int currentObjective;
	public string firstMessage;
	public string secondMessage;
	public string thirdMessage;
	public string fourthMessage;
	public string fifthMessage;
	public string deathMessage;
	public string finishedMessage;
	public GameObject powerToKillPlayer;
	public GameObject waringSign;
	private SceneFadeInOut transitionToNewScene;
	private bool endScene;
	private float fadeTimer;
	private float durationOfFading;
	private bool fade;
	private bool deathScreenEvent;
	private PlacementUI deathscreen;

	
	void Awake(){
		
	}
	
	// Use this for initialization
	void Start () {
		fade = false;
		durationOfFading = 1;
		fadeTimer = 1;
		transitionToNewScene = this.GetComponent<SceneFadeInOut> ();
		currentObjective = 0;
		objectives [currentObjective].SetActive (true);
		guiText = GameObject.Find ("HUDText").GetComponent<UILabel>();
		changeText( firstMessage );
		runScene(currentObjective);
		endScene = false;
		powerToKillPlayer.SetActive (false);
		deathScreenEvent = false;
	}
	

	void Update () {
		
		textAnimation ();
		
		if (endScene)
			loadNextLevel ();
		
		if (player == null && !deathScreenEvent )
			resetLevel ();
		
		incrementObjective();
	}

	//correctly applies fade animations for ui text
	private void textAnimation(){
		if (fade)
			fadeTextClear ();
		
		if (!fade)
			fadeTextSolid ();
	}

	//sets fade to make invisable
	private void setFadeClear(){
		fadeTimer = 0;
		fade = true;
	}

	//sets fade back to visable
	private void setFadeSolid(){
		fadeTimer = 0;
		fade = false;
	}

	//increases count to next objective for player to complete
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

	//changes ui Text
	private void changeText( string newText ){
		guiText.text = newText;
	}

	//runs specific events once objectives are completed
	private void runScene( int current ){
		switch( current ){
		case 0:
			Debug.Log("Start");
			StartCoroutine( startScene());
			break;
		case 1:
			Debug.Log("Active");

			StartCoroutine( teachActive() );
			break;
		case 2:
			Debug.Log("Passive");

			StartCoroutine( teachPassive() );
			break;
		case 3:
			Debug.Log("Death");

			StartCoroutine( teachDeathScreen() );
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
	
	//get fountain
	IEnumerator startScene(){
		player.GetComponent<Controller2D> ().locked = true;
		yield return new WaitForSeconds (durationOfFading);
		setFadeClear ();
		yield return new WaitForSeconds (durationOfFading);
		setFadeSolid ();
		changeText (secondMessage);
		player.GetComponent<Controller2D> ().locked = false;
		
	}
	
	//teach active
	//--flash active slot icon
	//--display text
	IEnumerator teachActive(){
		setFadeClear ();
		yield return new WaitForSeconds (durationOfFading);
		setFadeSolid ();
		changeText (thirdMessage);
		player.GetComponent<Controller2D> ().locked = false;


	}
	
	//teach passive
	//--flash passive icon
	//--display text
	IEnumerator teachPassive(){
		setFadeClear ();
		yield return new WaitForSeconds (durationOfFading);
		setFadeSolid ();
		changeText (fourthMessage);
		player.GetComponent<Controller2D> ().locked = false;			
	}

	//teach death screen
	//spawn a dummy and a power
	//resupply with  powers
	IEnumerator teachDeathScreen(){
		deathScreenEvent = true;
		deathscreen = GameObject.Find( "OfflinePlacement(Clone)").GetComponent<PlacementUI>();
		waringSign.transform.position = player.transform.position;
		waringSign.SetActive (true);
		yield return new WaitForSeconds (0.5f);
		player.GetComponent<Controller2D> ().locked = true;
		setupPowerToUse();
		yield return new WaitForSeconds (2f);
		setFadeClear ();
		yield return new WaitForSeconds (durationOfFading);
		setFadeSolid ();
		changeText (fifthMessage);
		InvokeRepeating ("resupplyDeathScreen", 1f, 10f);

	}

	public void setupPowerToUse(){
		powerToKillPlayer.transform.position = waringSign.transform.position;
		Destroy (waringSign);
		powerToKillPlayer.SetActive (true);
	}

	//loads next level when needed
	public void loadNextLevel(){
		changeText (finishedMessage);
		transitionToNewScene.EndScene( nextLevel );
	}

	public void resupplyDeathScreen(){
		deathscreen.Resupply();
	}

	//reset level
	public void resetLevel(){
		changeText (deathMessage);
		Application.LoadLevel (Application.loadedLevel);
	}
}

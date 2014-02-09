using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public GameObject ScoreLabel;
	public GameObject PlayerLabel;
	SessionManager sessionManager;
	public float timeleft = 5;
	PSinfo infoscript;

	// Use this for initialization
	void Start () {



	}

	//On Level Loaded tries to spawn the labels of player scores.

	void OnNetworkLoadedLevel(){


		sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
		infoscript = sessionManager.gameInfo;
		
		
		//Goes into server loop if hosting and else loop if not at the moment
		
		
		
		if(Network.isServer){
			print("well then");
			networkView.RPC("DisplayScores", RPCMode.Others);
		} else {
			
			networkView.RPC ("NotifyDisplayScores", RPCMode.Server);
			ShowScoresLocally();
		}

	}


	//Displays the labels with score and player info
	void ShowScoresLocally(){
		print ("gettting local");
			var scorelabel = (GameObject)Instantiate(ScoreLabel, new Vector2(0,0), transform.rotation);
			var playerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);
			
			scorelabel.transform.parent = GameObject.Find("UI Root").transform;
			playerlabel.transform.parent = GameObject.Find("UI Root").transform;
			scorelabel.transform.localScale = new Vector3(1, 1, 1);
			playerlabel.transform.localScale = new Vector3(1, 1, 1);
			scorelabel.transform.localPosition = new Vector2(-100, 0+(200*infoscript.playernumb));
			playerlabel.transform.localPosition = new Vector2(100, 0+(200*infoscript.playernumb));
			
			print (infoscript.playernumb);

			
			var scoretext = scorelabel.GetComponent<UILabel>();
			var playertext = playerlabel.GetComponent<UILabel>();
			scoretext.text = ((infoscript.score).ToString()) + " points";
			playertext.text = infoscript.playername;

	


	}


	[RPC]
	void NotifyDisplayScores(){

		networkView.RPC("DisplayScores", RPCMode.Others);
		ShowScoresLocally();
	}

	[RPC]
	void DisplayScores(){
		print ("MERRRP");
		ShowScoresLocally();


	}
	

	// Update is called once per frame
	void Update () {
	
		if(Network.isServer){

		if(timeleft > 0){

			timeleft -= Time.deltaTime;

		} else {


			sessionManager.LoadNextLevel(false);

		}
		}

	}
}

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
		var listed = infoscript.players;
		print(listed);
		print(listed.Count);
		print(infoscript.GetPlayerStats(listed[0]).score);

		for(int i = 0; i < listed.Count; i++){

			var score = infoscript.GetPlayerStats(listed[i]).score;
			var playername = infoscript.GetPlayerOptions(listed[i]).username;
			var playernumber = i + 1;
			ShowScoresLocally(score, playername, playernumber);
		}

		
		//Goes into server loop if hosting and else loop if not at the moment
		
		

	}


	//Displays the labels with score and player info
	void ShowScoresLocally(int score, string playername, int playernumber){
		print ("gettting local");
			var scorelabel = (GameObject)Instantiate(ScoreLabel, new Vector2(0,0), transform.rotation);
			var playerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);
			
			scorelabel.transform.parent = GameObject.Find("UI Root").transform;
			playerlabel.transform.parent = GameObject.Find("UI Root").transform;
			scorelabel.transform.localScale = new Vector3(1, 1, 1);
			playerlabel.transform.localScale = new Vector3(1, 1, 1);
			scorelabel.transform.localPosition = new Vector2(-100, 100+(-100*playernumber));
			playerlabel.transform.localPosition = new Vector2(100, 100+(-100*playernumber));
			
			print (infoscript.playernumb);

			
			var scoretext = scorelabel.GetComponent<UILabel>();
			var playertext = playerlabel.GetComponent<UILabel>();
			scoretext.text = score.ToString() + " points";
			playertext.text = playername;

	


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

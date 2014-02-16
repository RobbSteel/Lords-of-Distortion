using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public GameObject ScoreLabel;
	public GameObject PlayerLabel;
	SessionManager sessionManager;
	public float timeleft = 5;
	public int winningscore = 0;
	public int winningplayer = 0;
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


		//If the Match is finished check who was the winner
		if(sessionManager.matchfinish){
			print ("calculate winnings");
			for(int v = 0; v < listed.Count; v++){

				var score = infoscript.GetPlayerStats(listed[v]).score;
						if(score > winningscore){
							winningscore = score;
							winningplayer = v;
						}

			}
			
			
		}

		for(int i = 0; i < listed.Count; i++){

			var score = infoscript.GetPlayerStats(listed[i]).score;
			var playername = infoscript.GetPlayerOptions(listed[i]).username;
			var playernumber = i + 1;
			ShowScoresLocally(score, playername, playernumber);

		}

		//Wipes scores after displaying them if game is over.
		if(sessionManager.matchfinish){

			for(int z = 0; z < listed.Count; z++){
				infoscript.GetPlayerStats(listed[z]).score = 0;
			}

		}
		

	}


	//Displays the labels with score and player info
	void ShowScoresLocally(int score, string playername, int playernumber){

		//Used to print 3 losers and 1 winner
		if(sessionManager.matchfinish){
			print("Got to winnings");
			var scorelabel = (GameObject)Instantiate(ScoreLabel, new Vector2(0,0), transform.rotation);
			var playerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);

			scorelabel.transform.parent = GameObject.Find("UI Root").transform;
			playerlabel.transform.parent = GameObject.Find("UI Root").transform;


			scorelabel.transform.localScale = new Vector3(1, 1, 1);
			playerlabel.transform.localScale = new Vector3(1, 1, 1);

			//This one prints winner
			if(playernumber == (winningplayer + 1)){
			var winnerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);
			winnerlabel.transform.parent = GameObject.Find ("UI Root").transform;
			winnerlabel.transform.localScale = new Vector3(1, 1, 1);
			
			scorelabel.transform.localPosition = new Vector2(200, 0);
			playerlabel.transform.localPosition = new Vector2(400, 0);
			winnerlabel.transform.localPosition = new Vector2(300, 100);

			var wintext = winnerlabel.GetComponent<UILabel>();
			wintext.text = "Winner!";

			} else {
			//This one prints losers
			scorelabel.transform.localPosition = new Vector2(-444, 100+(-100*playernumber));
			playerlabel.transform.localPosition = new Vector2(-200, 100+(-100*playernumber));
			
			}

			print (infoscript.playernumb);
			
			
			var scoretext = scorelabel.GetComponent<UILabel>();
			var playertext = playerlabel.GetComponent<UILabel>();

			scoretext.text = score.ToString() + " points";
			playertext.text = playername;






		}else{
//This happens for every game where victory isnt being declared.
		print ("gettting local");
			var scorelabel = (GameObject)Instantiate(ScoreLabel, new Vector2(0,0), transform.rotation);
			var playerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);
			
			scorelabel.transform.parent = GameObject.Find("UI Root").transform;
			playerlabel.transform.parent = GameObject.Find("UI Root").transform;
			scorelabel.transform.localScale = new Vector3(1, 1, 1);
			playerlabel.transform.localScale = new Vector3(1, 1, 1);
			scorelabel.transform.localPosition = new Vector2(-444, 100+(-100*playernumber));
			playerlabel.transform.localPosition = new Vector2(-200, 100+(-100*playernumber));
			
			print (infoscript.playernumb);

			
			var scoretext = scorelabel.GetComponent<UILabel>();
			var playertext = playerlabel.GetComponent<UILabel>();
			scoretext.text = score.ToString() + " points";
			playertext.text = playername;

	
		}

	}


	
	bool sentLevelLoadRPC = false;
	// Update is called once per frame
	void Update () {
	
		if(Network.isServer){

		   if(timeleft > 0){

			   timeleft -= Time.deltaTime;
				 
		    } else if(!sentLevelLoadRPC){
			
			   sessionManager.LoadNextLevel(false);
			   sentLevelLoadRPC = true;
		    }
	   }
	}
}

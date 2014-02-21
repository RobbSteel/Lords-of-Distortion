﻿using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public GameObject ScoreLabel;
	public GameObject PlayerLabel;
	public GameObject PlayerDisplay;
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
			var playercolor = infoscript.GetPlayerOptions(listed[i]).style;
			print(playercolor);
			var playernumber = i + 1;
			ShowScoresLocally(score, playername, playernumber, playercolor);

		}

		//Wipes scores after displaying them if game is over.
		if(sessionManager.matchfinish){

			for(int z = 0; z < listed.Count; z++){
				sessionManager.matchfinish = false;
				infoscript.GetPlayerStats(listed[z]).score = 0;
			}

		}
		

	}

	//This happens for every game where victory isnt being declared
	void RoundFinish(int score, string playername, int playernumber, GameObject playerpose){

		print ("gettting local");
		var scorelabel = (GameObject)Instantiate(ScoreLabel, new Vector2(0,0), transform.rotation);
		var playerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);
		
		scorelabel.transform.parent = GameObject.Find("UI Root").transform;
		playerlabel.transform.parent = GameObject.Find("UI Root").transform;
		playerpose.transform.parent = GameObject.Find ("UI Root").transform;
		
		scorelabel.transform.localScale = new Vector3(1, 1, 1);
		playerlabel.transform.localScale = new Vector3(1, 1, 1);
		playerpose.transform.localScale = new Vector3(100,100,1);
		
		scorelabel.transform.localPosition = new Vector2(-444, 350+(-150*playernumber));
		playerlabel.transform.localPosition = new Vector2(-200, 350+(-150*playernumber));
		playerpose.transform.localPosition = new Vector2(0, 350+(-150*playernumber));
		
		print (infoscript.playernumb);
		
		
		var scoretext = scorelabel.GetComponent<UILabel>();
		var playertext = playerlabel.GetComponent<UILabel>();
		scoretext.text = score.ToString() + " points";
		playertext.text = playername;
		
		
	}

	//This declares a winner
	void MatchFinish(int score, string playername, int playernumber, GameObject playerpose){

		print("Got to winnings");
		var scorelabel = (GameObject)Instantiate(ScoreLabel, new Vector2(0,0), transform.rotation);
		var playerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);
		
		
		scorelabel.transform.parent = GameObject.Find("UI Root").transform;
		playerlabel.transform.parent = GameObject.Find("UI Root").transform;
		playerpose.transform.parent = GameObject.Find ("UI Root").transform;
		
		scorelabel.transform.localScale = new Vector3(1, 1, 1);
		playerlabel.transform.localScale = new Vector3(1, 1, 1);
		playerpose.transform.localScale = new Vector3(100,100,1);
		
		//This one prints winner
		if(playernumber == (winningplayer + 1)){
			var winnerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);
			winnerlabel.transform.parent = GameObject.Find ("UI Root").transform;
			winnerlabel.transform.localScale = new Vector3(1, 1, 1);
			
			scorelabel.transform.localPosition = new Vector2(150, -100);
			playerlabel.transform.localPosition = new Vector2(400, -100);
			winnerlabel.transform.localPosition = new Vector2(275, 0);
			playerpose.transform.localPosition = new Vector2(275, 150);
			
			var wintext = winnerlabel.GetComponent<UILabel>();
			wintext.text = "Winner!";
			
		} else {
			//This one prints losers
			scorelabel.transform.localPosition = new Vector2(-444, 350+(-150*playernumber));
			playerlabel.transform.localPosition = new Vector2(-200, 350+(-150*playernumber));
			playerpose.transform.localPosition = new Vector2(0, 350+(-150*playernumber));
		}
		
		print (infoscript.playernumb);
		
		
		var scoretext = scorelabel.GetComponent<UILabel>();
		var playertext = playerlabel.GetComponent<UILabel>();
		
		scoretext.text = score.ToString() + " points";
		playertext.text = playername;
		

	}


	//Displays the labels with score and player info
	void ShowScoresLocally(int score, string playername, int playernumber, PlayerOptions.CharacterStyle playercolor){

		string color = "white";

		//Checks for the current color of the player who's score is being displayed.
		if(playercolor == PlayerOptions.CharacterStyle.DEFAULT){
			color = "white";
		} else if(playercolor == PlayerOptions.CharacterStyle.RED){
			color = "red";
		} else if(playercolor == PlayerOptions.CharacterStyle.GREEN){
			color = "green";
		} else if(playercolor == PlayerOptions.CharacterStyle.BLUE){
			color = "blue";
		}

		var playerpose = DetermineColor(color);


		if(sessionManager.matchfinish){
		
			 MatchFinish(score, playername, playernumber, playerpose);
		  }else{

			 RoundFinish(score,playername, playernumber, playerpose);
          }
		}

	//Instantiate the score pose with the appropriate color and returns it to displaylocally.
	GameObject DetermineColor(string color){

		GameObject tempplayer;

		if(color == "white"){

			tempplayer = (GameObject)Instantiate(PlayerDisplay, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.white;
			return tempplayer;
		}

		if(color == "red"){
			
			tempplayer = (GameObject)Instantiate(PlayerDisplay, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.red;
			return tempplayer;
		}

		if(color == "green"){
			
			tempplayer = (GameObject)Instantiate(PlayerDisplay, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.green;
			return tempplayer;
		}

		if(color == "blue"){
			
			tempplayer = (GameObject)Instantiate(PlayerDisplay, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.blue;
			return tempplayer;
		}

		tempplayer = (GameObject)Instantiate(PlayerDisplay, new Vector2(0,0), transform.rotation);
		return tempplayer; 
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

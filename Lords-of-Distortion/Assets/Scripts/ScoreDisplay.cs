using UnityEngine;
using System.Collections;
using Priority_Queue;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour {

	public GameObject ScoreLabel;
	public GameObject PlayerLabel;
	public GameObject PlayerDisplay;
	public GameObject BluePlayerDisplay;
	public GameObject BlueEnd;
	public GameObject ColEnd;
	public GameObject KillLabel;
	public GameObject AssistLabel;
	public GameObject WinLabel;
	public GameObject FavorLabel;
	public GameObject TimeLabel;
	public List<NetworkPlayer> tielist;
	public bool tie = false;
	public bool finish = false;
	SessionManager sessionManager;
	public float timeleft;
	public float winningscore = -1;
	public float gonethrough = 0;
	public NetworkPlayer winningplayer;

	//Different Death Icons
	public GameObject electricicon;
	public GameObject plagueicon;
	public GameObject hookicon;
	public GameObject stickyicon;
	public GameObject iceicon;
	public GameObject fireicon;
	public GameObject spikeicon;
	public GameObject bouldericon;
	public GameObject blackholeicon;
	public GameObject earthicon;
	public GameObject alive;
	public GameObject deflecticon;

	PlayerServerInfo infoscript;
	private string timeLabelDescription;
	private GameObject timeLabelReference;

	// Use this for initialization
	void Start () {
	}

	//On Level Loaded tries to spawn the labels of player scores.

	void OnNetworkLoadedLevel(){

		sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
		infoscript = sessionManager.psInfo;
		var listed = infoscript.players;
		print(listed);
		print(listed.Count);
		print(infoscript.GetPlayerStats(listed[0]).roundScore);

		if(sessionManager.matchfinish){
			timeleft = 12;
			timeLabelDescription = "Returning to Lobby: ";
			var timelabel = (GameObject)Instantiate(TimeLabel, new Vector2(0,0), transform.rotation);
			timelabel.transform.parent = GameObject.Find("UI Root").transform;
			timelabel.transform.localScale = new Vector3(1,1,1);
			timelabel.transform.localPosition = new Vector2 (-590,285);
			//timeLabelReference.GetComponent<UILabel>().text =  timeLabelDescription + Mathf.CeilToInt( timeleft ).ToString();
			timeLabelReference = timelabel;

		} else {
			timeleft = 6;
			timeLabelDescription = "Next Round in: ";
			var timelabel = (GameObject)Instantiate (TimeLabel, new Vector2 (0, 0), transform.rotation);
			timelabel.transform.parent = GameObject.Find ("UI Root").transform;
			timelabel.transform.localScale = new Vector3(1,1,1);
			timelabel.transform.localPosition = new Vector2 (-590,285);
			timeLabelReference = timelabel;
		}

		//If the Match is finished check who was the winner and set up tie logic
		if(sessionManager.matchfinish){
		
			for(int v = 0; v < listed.Count; v++){

				var score = infoscript.GetPlayerStats(listed[v]).totalScore;
						
				        if(score > winningscore){
							winningscore = score;
							winningplayer = listed[v];
						} 
			}

		}

		//Calculate scores for each player
		for(int i = 0; i < listed.Count; i++){

			var roundscore = infoscript.GetPlayerStats(listed[i]).roundScore;
			var totalscore = infoscript.GetPlayerStats(listed[i]).totalScore;

			PowerType lastdeath = PowerType.UNDEFINED;

			PlayerStats stats = infoscript.GetPlayerStats(listed[i]);

			//only want last event if player died.
			if(stats.isDead()){
				lastdeath = stats.LastEvent().PowerType;
			}

			var playername = infoscript.GetPlayerOptions(listed[i]).username;
			var playermodel = infoscript.GetPlayerOptions(listed[i]).character;
			var playercolor = infoscript.GetPlayerOptions(listed[i]).style;
			var playernumber = i + 1;
			ShowScoresLocally(roundscore, totalscore, playername, playernumber, playercolor, lastdeath, playermodel);

		}

		//Wipes scores in playerstats after displaying them if the game is over.
		if(sessionManager.matchfinish){

			for(int z = 0; z < listed.Count; z++){

				infoscript.GetPlayerStats(listed[z]).totalScore = 0;
			}
			sessionManager.matchfinish = false;
		}
		

	}

	//Visually display death for players
	void DisplayDeath(PowerType lastdeath, int playernumber){


		if(lastdeath != PowerType.UNDEFINED){
			GameObject label;
			if(PowerType.ELECTRIC == lastdeath){
				label = (GameObject)Instantiate(electricicon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.BOULDER == lastdeath){
				label = (GameObject)Instantiate(bouldericon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.EARTH == lastdeath){
				label = (GameObject)Instantiate(earthicon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.EXPLOSIVE == lastdeath){
				label = (GameObject)Instantiate(stickyicon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.FIREBALL == lastdeath){
				label = (GameObject)Instantiate(fireicon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.FREEZE == lastdeath){
				label = (GameObject)Instantiate(iceicon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.POWERHOOK == lastdeath){
				label = (GameObject)Instantiate(hookicon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.BH_INNER == lastdeath){
				label = (GameObject)Instantiate(blackholeicon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.PLAGUE == lastdeath){
				label = (GameObject)Instantiate(plagueicon, new Vector2(0,0), transform.rotation);
			}

			else if(PowerType.SPIKES == lastdeath){
				label = (GameObject)Instantiate(spikeicon, new Vector2(0,0), transform.rotation);
			

			}else if(PowerType.DEFLECTIVE == lastdeath){
					label = (GameObject)Instantiate(deflecticon, new Vector2(0,0), transform.rotation);
				
			} else {

				label = (GameObject)Instantiate(alive, new Vector2(0,0), transform.rotation);
			}

			label.transform.parent = GameObject.Find("UI Root").transform;
			label.transform.localScale = new Vector3(80,80,1);
			label.transform.localPosition = new Vector2(800, 1800+(-800*playernumber));
		}
	

	}

	//This happens for every game where victory isnt being declared
	void RoundFinish(float roundscore, float totalscore, string playername, int playernumber, GameObject playerpose, PowerType lastdeath){

		DisplayDeath(lastdeath, playernumber);

		//Instantiate Label Objects
		var playerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);
		var killlabel = (GameObject)Instantiate(KillLabel, new Vector2(0,0), transform.rotation);
		var favorlabel = (GameObject)Instantiate(FavorLabel, new Vector2(0,0), transform.rotation);


		//Add them to the UI
		playerlabel.transform.parent = GameObject.Find("UI Root").transform;
		playerpose.transform.parent = GameObject.Find ("UI Root").transform;
		killlabel.transform.parent = GameObject.Find("UI Root").transform;
		favorlabel.transform.parent = GameObject.Find("UI Root").transform;


		//Rescale them to fit properly
		playerlabel.transform.localScale = new Vector3(1, 1, 1);
		playerpose.transform.localScale = new Vector3(100,100,1);
		favorlabel.transform.localScale = new Vector3(1,1,1);
		killlabel.transform.localScale = new Vector3(1,1,1);


		//Locate them to the proper locations
		playerlabel.transform.localPosition = new Vector2(-444, 350+(-150*playernumber));
		playerpose.transform.localPosition = new Vector2(-200, 350+(-150*playernumber));
		killlabel.transform.localPosition = new Vector2(350, 350+(-150*playernumber));
		favorlabel.transform.localPosition = new Vector2(520, 350+(-150*playernumber));


		//Find the Text component
		var playertext = playerlabel.GetComponent<UILabel>();
		var killstext = killlabel.GetComponent<UILabel>();
		var favortext = favorlabel.GetComponent<UILabel>();

		//Add score text to the box
		playertext.text = playername;
		killstext.text = "+" + roundscore;
		favortext.text = "" + totalscore;
		//need ref to time label since it is constantly updated 


		
	}

	string ColorCheck(PlayerOptions.CharacterStyle playercolor){
		string color = "white";
		if(playercolor == PlayerOptions.CharacterStyle.DEFAULT){
			color = "white";
		} else if(playercolor == PlayerOptions.CharacterStyle.RED){
			color = "red";
		} else if(playercolor == PlayerOptions.CharacterStyle.GREEN){
			color = "green";
		} else if(playercolor == PlayerOptions.CharacterStyle.BLUE){
			color = "blue";
		}

		return color;
	}

	//Displays the labels with score and player info
	void ShowScoresLocally(float roundscore, float totalscore, string playername, int playernumber, PlayerOptions.CharacterStyle playercolor, PowerType lastdeath, PlayerOptions.Character playermodel){



		string color = ColorCheck(playercolor);


		print ("blue char");
		GameObject playerpose;
	if(playermodel == PlayerOptions.Character.Blue){

			 playerpose = DetermineColorBlue(color);
		} else {

			 playerpose = DetermineColor(color);
		}

		RoundFinish(roundscore, totalscore, playername, playernumber, playerpose, lastdeath);

          
		}

	void MatchFinish(PlayerServerInfo infoscript){




		if(tie){

			for(int i = 0; i < tielist.Count; i++){
				var playername = infoscript.GetPlayerOptions(tielist[i]).username;
				var playercolor = infoscript.GetPlayerOptions(tielist[i]).style;
				var playerclr = ColorCheck(playercolor);
				var playericon = DetermineColor(playerclr);
				playericon.transform.parent = GameObject.Find ("UI Root").transform;
				playericon.transform.localScale = new Vector3(100,100,1);
				playericon.transform.localPosition = new Vector2(-200, 350+(-150));


			}

		} else {


			var currplayers = infoscript.players;
			var loselabel = (GameObject)Instantiate(WinLabel, new Vector2(0,0), transform.rotation);
			loselabel.transform.parent = GameObject.Find ("UI Root").transform;
			loselabel.transform.localScale = new Vector3(1,1,1);
			loselabel.transform.localPosition = new Vector2(300, 300);
			var losetext = loselabel.GetComponent<UILabel>();
			losetext.text = "LOSERS";

		




			for(int i = 0; i < currplayers.Count; i++){

				if(currplayers[i] == winningplayer){

					var playername = infoscript.GetPlayerOptions(winningplayer).username;
					var playercolor = infoscript.GetPlayerOptions(winningplayer).style;
					var playertype = infoscript.GetPlayerOptions(currplayers[i]).character;
					var playerclr = ColorCheck(playercolor);

					GameObject playericon;
					if(playertype == PlayerOptions.Character.Blue){
						playericon = DetermineColorBlueEnd(playerclr);
					} else {
						playericon = DetermineColorEnd(playerclr);
					}

					var playerlabel = (GameObject)Instantiate(WinLabel, new Vector2(0,0), transform.rotation);
					var winlabel = (GameObject)Instantiate(WinLabel, new Vector2(0,0), transform.rotation);
					
					playerlabel.transform.parent = GameObject.Find ("UI Root").transform;
					playericon.transform.parent = GameObject.Find ("UI Root").transform;
					winlabel.transform.parent = GameObject.Find ("UI Root").transform;
					
					playerlabel.transform.localScale = new Vector3(1, 1, 1);
					playericon.transform.localScale = new Vector3(200,200,1);
					winlabel.transform.localScale = new Vector3(1,1,1);
					
					playerlabel.transform.localPosition = new Vector2(-400, 350+(-500));
					playericon.transform.localPosition = new Vector2(-400, 350+(-300));
					winlabel.transform.localPosition = new Vector2(-400, 350+(-100));
					
					var playertext = playerlabel.GetComponent<UILabel>();
					var wintext = winlabel.GetComponent<UILabel>();
					
					playertext.text = playername;
					wintext.text = "WINNER!";

			} else {

					var playername = infoscript.GetPlayerOptions(currplayers[i]).username;
					var playercolor = infoscript.GetPlayerOptions(currplayers[i]).style;
					var playertype = infoscript.GetPlayerOptions(currplayers[i]).character;
					var playerclr = ColorCheck(playercolor);
					GameObject playericon;
						if(playertype == PlayerOptions.Character.Blue){
							playericon = DetermineColorBlue(playerclr);
						} else {
							playericon = DetermineColor(playerclr);
						}
					
					var playerlabel = (GameObject)Instantiate(WinLabel, new Vector2(0,0), transform.rotation);

					
					playerlabel.transform.parent = GameObject.Find ("UI Root").transform;
					playericon.transform.parent = GameObject.Find ("UI Root").transform;


					playerlabel.transform.localScale = new Vector3(1f, 1f, 1);
					playericon.transform.localScale = new Vector3(100,100,1);

					
					playerlabel.transform.localPosition = new Vector2(200, 150+(-200 * gonethrough));
					playericon.transform.localPosition = new Vector2(400, 150+(-200 * gonethrough));

					
					var playertext = playerlabel.GetComponent<UILabel>();

					
					playertext.text = playername;
					gonethrough++;



			}


		
		}

	}
	}

	GameObject DetermineColorBlueEnd(string color){
		
		GameObject tempplayer;
		
		if(color == "white"){
			
			tempplayer = (GameObject)Instantiate(BlueEnd, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.white;
			return tempplayer;
		}
		
		if(color == "red"){
			
			tempplayer = (GameObject)Instantiate(BlueEnd, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.red;
			return tempplayer;
		}
		
		if(color == "green"){
			
			tempplayer = (GameObject)Instantiate(BlueEnd, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.green;
			return tempplayer;
		}
		
		if(color == "blue"){
			
			tempplayer = (GameObject)Instantiate(BlueEnd, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.blue;
			return tempplayer;
		}
		
		tempplayer = (GameObject)Instantiate(BlueEnd, new Vector2(0,0), transform.rotation);
		return tempplayer; 
	}



	GameObject DetermineColorEnd(string color){
		
		GameObject tempplayer;
		
		if(color == "white"){
			
			tempplayer = (GameObject)Instantiate(ColEnd, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.white;
			return tempplayer;
		}
		
		if(color == "red"){
			
			tempplayer = (GameObject)Instantiate(ColEnd, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.red;
			return tempplayer;
		}
		
		if(color == "green"){
			
			tempplayer = (GameObject)Instantiate(ColEnd, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.green;
			return tempplayer;
		}
		
		if(color == "blue"){
			
			tempplayer = (GameObject)Instantiate(ColEnd, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.blue;
			return tempplayer;
		}
		
		tempplayer = (GameObject)Instantiate(ColEnd, new Vector2(0,0), transform.rotation);
		return tempplayer; 
	}

	//Instantiate the score pose with the appropriate color for BLUE character and returns it to displaylocally.
	GameObject DetermineColorBlue(string color){
		
		GameObject tempplayer;
		
		if(color == "white"){
			
			tempplayer = (GameObject)Instantiate(BluePlayerDisplay, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.white;
			return tempplayer;
		}
		
		if(color == "red"){
			
			tempplayer = (GameObject)Instantiate(BluePlayerDisplay, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.red;
			return tempplayer;
		}
		
		if(color == "green"){
			
			tempplayer = (GameObject)Instantiate(BluePlayerDisplay, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.green;
			return tempplayer;
		}
		
		if(color == "blue"){
			
			tempplayer = (GameObject)Instantiate(BluePlayerDisplay, new Vector2(0,0), transform.rotation);
			var tempcolor = tempplayer.GetComponent<SpriteRenderer>();
			tempcolor.color = Color.blue;
			return tempplayer;
		}
		
		tempplayer = (GameObject)Instantiate(PlayerDisplay, new Vector2(0,0), transform.rotation);
		return tempplayer; 
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

		if(timeleft < 7 && timeleft > 6 && !finish){
			print ("finish");
			var destroylist = GameObject.FindGameObjectsWithTag("ScoreLabels");

			for(int i = 0; i < destroylist.Length; i++){
				GameObject objectdestroy = destroylist[i];
				Destroy(objectdestroy);

			}
		
			MatchFinish(infoscript);
			finish = true;
		}




		   if(timeleft > 0){

			timeleft -= Time.deltaTime;

			//display correct time text
			if( timeLabelReference != null )
			timeLabelReference.GetComponent<UILabel>().text =  timeLabelDescription + Mathf.CeilToInt( timeleft ).ToString();

		    } else if(!sentLevelLoadRPC){

				if(Network.isServer){
			   		sessionManager.LoadNextLevel(false);
			   		sentLevelLoadRPC = true;
	  		 	}
			}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LivesUI : MonoBehaviour {

	public GameObject PlayerLivesPrefab;
	public GameObject heartParticles;
	public Texture finalchar;

	//Palette Swaps
	//Blue
	public Texture BlueBlue;
	public Texture BlueRed;
	public Texture BlueGreen;
	public Texture BlueYellow;
	//Colossus
	public Texture ColBlue;
	public Texture ColRed;
	public Texture ColGreen;
	public Texture ColYellow;
	//Mummy
	public Texture MumBlue;
	public Texture MumRed;
	public Texture MumGreen;
	public Texture MumYellow;

	public Camera UICamera;

	public UIGrid livesGrid;

	PlayerServerInfo psInfo;
	
	Dictionary<NetworkPlayer, GameObject> entry  = new Dictionary<NetworkPlayer, GameObject>();

	string ColorCheck(CharacterStyle playercolor){
		string color = "white";
		if(playercolor == CharacterStyle.YELLOW){
			color = "yellow";
		} else if(playercolor == CharacterStyle.RED){
			color = "red";
		} else if(playercolor == CharacterStyle.GREEN){
			color = "green";
		} else if(playercolor == CharacterStyle.BLUE){
			color = "blue";
		}
		
		return color;
	}

	Texture MummyColorCheck(string color){
		
		Texture tempplayer;
		
		if(color == "yellow"){
			tempplayer = MumYellow;
			return tempplayer;
		}
		
		if(color == "red"){
			tempplayer = MumRed;
			return tempplayer;
		}
		
		if(color == "green"){
			tempplayer = MumGreen;
			return tempplayer;
		}
		
		else{
			tempplayer = MumBlue;
			return tempplayer;
		}
		
		
		
	}

	Texture ColColorCheck(string color){
		
		Texture tempplayer;
		
		if(color == "yellow"){
			tempplayer = ColYellow;
			return tempplayer;
		}
		
		if(color == "red"){
			tempplayer = ColRed;
			return tempplayer;
		}
		
		if(color == "green"){
			tempplayer = ColGreen;
			return tempplayer;
		}
		
		else{
			tempplayer = ColBlue;
			return tempplayer;
		}
		
		
		
	}

	Texture BlueColorCheck(string color){
		
		Texture tempplayer;
		
		if(color == "yellow"){
			tempplayer = BlueYellow;
			return tempplayer;
		}
		
		if(color == "red"){
			tempplayer = BlueRed;
			return tempplayer;
		}
		
		if(color == "green"){
			tempplayer = BlueGreen;
			return tempplayer;
		}
		
		else{
			tempplayer = BlueBlue;
			return tempplayer;
		}
		
		
		
	}

	//
	void DetermineColor(string color, Character playchar, GameObject playeravatar){
		GameObject tempplayer = playeravatar;
		var playertex = playeravatar.GetComponent<UITexture>().mainTexture;

		if(playchar == Character.Blue){
			finalchar = BlueColorCheck(color);
		}else if(playchar == Character.Mummy){
			finalchar = MummyColorCheck(color);
		} else {
			finalchar = ColColorCheck(color);
		}

		playeravatar.GetComponent<UITexture>().mainTexture = finalchar;

	}

	//Uses psinfo to generate player score grid.
	public void Initialize(PlayerServerInfo psInfo, float lives){
		this.psInfo = psInfo;

		foreach(NetworkPlayer player in psInfo.players){
			GameObject livesUI = NGUITools.AddChild(livesGrid.gameObject, PlayerLivesPrefab) as GameObject;
			livesUI.transform.localEulerAngles = new Vector3(0f, 0f, 3f);
			GameObject childLabel = livesUI.transform.GetChild(0).gameObject;
			childLabel.GetComponent<UILabel>().text = lives.ToString();
			GameObject playeravatar = childLabel.transform.GetChild(0).gameObject;

			var playertype = ColorCheck(psInfo.GetPlayerOptions(player).style);
			var playerchar = psInfo.GetPlayerOptions(player).character;
			DetermineColor(playertype, playerchar, playeravatar);
			entry.Add(player, livesUI);
		}
	}


	private void SetLives(GameObject livesUI, float lives){
		GameObject childLabel = livesUI.transform.GetChild(0).gameObject;
		childLabel.GetComponent<UILabel>().text = lives.ToString();
		TweenScale tween = childLabel.GetComponent<TweenScale>();
		tween.ResetToBeginning();
		tween.PlayForward();
	}

	public void DecreaseLives(NetworkPlayer player, float lives){

		GameObject playerEntry = entry[player];

		SetLives(playerEntry, lives);
		
	}



}

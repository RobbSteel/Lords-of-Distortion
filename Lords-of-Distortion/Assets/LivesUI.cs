using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LivesUI : MonoBehaviour {

	public GameObject PlayerLivesPrefab;
	public GameObject heartParticles;

	public Camera UICamera;

	public UIGrid livesGrid;

	PlayerServerInfo psInfo;
	
	Dictionary<NetworkPlayer, GameObject> entry  = new Dictionary<NetworkPlayer, GameObject>();

	//Uses psinfo to generate player score grid.
	public void Initialize(PlayerServerInfo psInfo, float lives){
		this.psInfo = psInfo;

		foreach(NetworkPlayer player in psInfo.players){
			GameObject livesUI = NGUITools.AddChild(livesGrid.gameObject, PlayerLivesPrefab) as GameObject;
			livesUI.transform.localEulerAngles = new Vector3(0f, 0f, 3f);
			GameObject childLabel = livesUI.transform.GetChild(0).gameObject;
			childLabel.GetComponent<UILabel>().text = lives.ToString();
			string playerName = psInfo.GetPlayerOptions(player).username;
			livesUI.GetComponent<UILabel>().text = playerName;
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

		/* World position can be obtained from NGUI widgets.
		 * If you grab NGUI camera, you can use worldToScreen to get screen coordinates.
		 * Once you have those, use can use ScreenToWorld on main camera to get world position

		//Doesnt look too good, removed
		Vector3 position = playerEntry.transform.position;
		position = UICamera.WorldToScreenPoint(position);
		position = Camera.main.ScreenToWorldPoint(position);
		Instantiate(heartParticles, position, Quaternion.identity);
		*/
	}

	/*
	public void Refresh(){
		foreach(var player in entries.Keys){
			SetScore(entries[player], psInfo.GetPlayerStats(player).roundScore);
		}
	}
	*/
}

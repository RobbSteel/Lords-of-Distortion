using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreUI : MonoBehaviour {

	public GameObject PlayerScorePrefab;
	public GameObject heartParticles;

	public Camera UICamera;

	public UIGrid scoresGrid;

	PlayerServerInfo psInfo;
	
	Dictionary<NetworkPlayer, GameObject> entries  = new Dictionary<NetworkPlayer, GameObject>();

	//Uses psinfo to generate player score grid.
	public void Initialize(PlayerServerInfo psInfo){
		this.psInfo = psInfo;

		foreach(NetworkPlayer player in psInfo.players){
			GameObject scoreUI = NGUITools.AddChild(scoresGrid.gameObject, PlayerScorePrefab) as GameObject;
			scoreUI.transform.localEulerAngles = new Vector3(0f, 0f, 3f);
			string playerName = psInfo.GetPlayerOptions(player).username;
			scoreUI.GetComponent<UILabel>().text = playerName;
			entries.Add(player, scoreUI);
		}
	}


	private void SetScore(GameObject scoreUI, float score){
		GameObject childLabel = scoreUI.transform.GetChild(0).gameObject;
		childLabel.GetComponent<UILabel>().text = score.ToString();
		TweenScale tween = childLabel.GetComponent<TweenScale>();
		tween.ResetToBeginning();
		tween.PlayForward();
	}

	public void IncreasePoints(NetworkPlayer player){

		GameObject playerEntry = entries[player];

		SetScore(playerEntry, psInfo.GetPlayerStats(player).roundScore);

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

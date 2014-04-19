using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreUI : MonoBehaviour {

	public GameObject PlayerScorePrefab;

	public UIGrid scoresGrid;

	PlayerServerInfo psInfo;
	
	Dictionary<NetworkPlayer, GameObject> entries  = new Dictionary<NetworkPlayer, GameObject>();

	//Uses psinfo to generate player score grid.
	public void Initialize(PlayerServerInfo psInfo){
		this.psInfo = psInfo;

		foreach(NetworkPlayer player in psInfo.players){
			GameObject scoreUI = NGUITools.AddChild(scoresGrid.gameObject, PlayerScorePrefab) as GameObject;
			string playerName = psInfo.GetPlayerOptions(player).username;
			scoreUI.GetComponent<UILabel>().text = playerName;
			entries.Add(player, scoreUI);
		}
	}


	private void SetScore(GameObject scoreUI, float score){
		//TODO: have a more refined way of doing this
		GameObject childLabel = scoreUI.transform.GetChild(0).gameObject;
		childLabel.GetComponent<UILabel>().text = score.ToString();
	}

	public void Refresh(){
		foreach(var player in entries.Keys){
			SetScore(entries[player], psInfo.GetPlayerStats(player).roundScore);
		}
	}
}

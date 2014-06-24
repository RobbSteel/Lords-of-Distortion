using UnityEngine;
using System.Collections;

public class PopulateMatches : MonoBehaviour {

	
	public GameObject playertitle;
	public GameObject serverlabel;
	public PlayerServerInfo infoscript;

	// Use this for initialization
	void Start () {
		playertitle = GameObject.Find("PlayerName");
		infoscript = PlayerServerInfo.Instance;
		//UILabel playerlabel = playertitle.GetComponentInChildren<UILabel>();
		//playerlabel.text = infoscript.localOptions.username;
	}
	
	// Update is called once per frame
	void Update () {
		//print(infoscript.playername);
	}
}

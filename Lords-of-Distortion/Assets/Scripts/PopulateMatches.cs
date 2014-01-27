using UnityEngine;
using System.Collections;

public class PopulateMatches : MonoBehaviour {


	public GameObject information;
	public GameObject playertitle;
	public GameObject serverlabel;
	public PSinfo infoscript;

	// Use this for initialization
	void Start () {
		playertitle = GameObject.Find("PlayerName");
		information = GameObject.Find("PSInfo");
		infoscript = information.GetComponent<PSinfo>();
		UILabel playerlabel = playertitle.GetComponentInChildren<UILabel>();
		playerlabel.text = infoscript.playername;
	}
	
	// Update is called once per frame
	void Update () {
		print(infoscript.playername);
	}
}

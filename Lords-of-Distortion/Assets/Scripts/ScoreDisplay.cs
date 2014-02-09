using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public GameObject ScoreLabel;
	public GameObject PlayerLabel;
	SessionManager sessionManager;
	public float timeleft = 5;


	// Use this for initialization
	void Start () {

		sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();
		var PSinfo = GameObject.Find("PSInfo");
		var infoscript = PSinfo.GetComponent<PSinfo>();

		var scorelabel = (GameObject)Instantiate(ScoreLabel, new Vector2(0,0), transform.rotation);
		var playerlabel = (GameObject)Instantiate(PlayerLabel, new Vector2(0,0), transform.rotation);

		scorelabel.transform.parent = GameObject.Find("UI Root").transform;
		playerlabel.transform.parent = GameObject.Find("UI Root").transform;
		scorelabel.transform.localScale = new Vector3(1, 1, 1);
		playerlabel.transform.localScale = new Vector3(1, 1, 1);
		scorelabel.transform.localPosition = new Vector2(-100, 0+(100*infoscript.playernumb));
		playerlabel.transform.localPosition = new Vector2(100, 0+(50*infoscript.playernumb));

		var scoretext = scorelabel.GetComponent<UILabel>();
		var playertext = playerlabel.GetComponent<UILabel>();
		scoretext.text = ((infoscript.score).ToString()) + " points";
		playertext.text = infoscript.playername;
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

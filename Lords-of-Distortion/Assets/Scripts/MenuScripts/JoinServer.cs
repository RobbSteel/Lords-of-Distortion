using UnityEngine;
using System.Collections;

public class JoinServer : MonoBehaviour {

	public int servernumber;
	public HostData[] hostList;
	public PSinfo infoscript;

	// Use this for initialization
	void Start () {
		var information = GameObject.Find("PSInfo");
		infoscript = information.GetComponent<PSinfo>();
	}

	void OnClick(){

		print (hostList);
		infoscript.choice = "Find";
		infoscript.servernumb = servernumber;
		infoscript.servername = hostList[servernumber].gameName;
		Application.LoadLevel("LobbyArena");
	}

	// Update is called once per frame
	void Update () {
	
	}
}

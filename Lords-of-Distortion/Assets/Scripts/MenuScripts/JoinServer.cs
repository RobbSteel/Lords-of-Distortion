using UnityEngine;
using System.Collections;


public class JoinServer : MonoBehaviour {

	public int servernumber;
	public HostData[] hostList;
	public PlayerServerInfo infoscript;

	// Use this for initialization
	void Start () {
		var information = GameObject.Find("PSInfo");
		infoscript = information.GetComponent<PlayerServerInfo>();
	}

	void OnPress(bool isDown){
		//do once only
		if(isDown)
			return;

		print (hostList[servernumber].comment);
		if(hostList[servernumber].comment == "InProgress"){
			print("Can't join game in progress");
		}
		else if(hostList[servernumber].connectedPlayers == 3){
			print ("There's too many players");
		}
		else{
			infoscript.choice = "Find";
			infoscript.chosenHost = hostList[servernumber];
			infoscript.servername = hostList[servernumber].gameName;
			var error = Network.Connect(hostList[servernumber]); //Attempt connecting, even though we might not have latest hostdata

		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnFailedToConnect(NetworkConnectionError error){
		print (error);
		//TODO Update label to show that the game is in progress (even though the error is too many players)
	}
	void OnDisconnectedFromServer(){
		//Actually load lobby were we connect for realsies
		Application.LoadLevel("LobbyArena");
	}

	void OnConnectedToServer(){
		Network.Disconnect();


	}

}

using UnityEngine;
using System.Collections;

public class JoinServer : MonoBehaviour {
	
	public HostData hostData;
	public PlayerServerInfo infoscript;

	// Use this for initialization
	void Start () {
		var information = GameObject.Find("PSInfo");
		infoscript = information.GetComponent<PlayerServerInfo>();

	}

	public void CheckButton()
	{
		UIButton button = GetComponent<UIButton>();
		if(hostData.comment == "Playing")
			button.isEnabled = false;
		else if(hostData.connectedPlayers >= 3)
			button.isEnabled = false;
	}

	void OnPress(bool isDown){
		//do once only
		if(isDown)
			return;

		print (hostData.comment);
		if(hostData.comment == "Playing"){
			print("Can't join game in progress");
		}
		else if(hostData.connectedPlayers == 3){
			print ("There's too many players");
		}
		else{
			infoscript.choice = "Find";
			infoscript.chosenHost = hostData;
			infoscript.servername = hostData.gameName;
			var error = Network.Connect(hostData); //Attempt connecting, even though we might not have latest hostdata

		}
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

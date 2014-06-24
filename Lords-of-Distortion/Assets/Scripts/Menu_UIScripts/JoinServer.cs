using UnityEngine;
using System.Collections;

public class JoinServer : MonoBehaviour {
	
	public HostData hostData;

	public void CheckButton()
	{
		UIButton button = GetComponent<UIButton>();
		if(hostData.comment == "Playing")
			button.isEnabled = false;
		else if(hostData.connectedPlayers > 3)
			button.isEnabled = false;
	}

	bool askedForConnection = false;

	void OnPress(bool isDown){
		//do once only
		if(isDown)
			return;

		print (hostData.comment);
		if(hostData.comment == "Playing"){
			print("Can't join game in progress");
		}
		else if(hostData.connectedPlayers > 3){
			print ("There's too many players");
		}
		else{
			askedForConnection = true;
			PlayerServerInfo.Instance.choice = "Find";
			PlayerServerInfo.Instance.chosenHost = hostData;
			PlayerServerInfo.Instance.servername = hostData.gameName;
			var error = Network.Connect(hostData); //Attempt connecting, even though we might not have latest hostdata
		}
	}
	
	void OnFailedToConnect(NetworkConnectionError error){
		if(!askedForConnection)
			return;
		print (error);

		//TODO Update label to show that the game is in progress (even though the error is too many players)
	}
	
	void OnDisconnectedFromServer(){
		if(!askedForConnection)
			return;
		askedForConnection = false;
		//Actually load lobby were we connect for realsies
		Application.LoadLevel("LobbyArena");
	}

	void OnConnectedToServer(){
		if(!askedForConnection)
			return;
		Network.Disconnect();
	}
}

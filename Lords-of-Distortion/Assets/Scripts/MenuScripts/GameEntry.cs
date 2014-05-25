using UnityEngine;
using System.Collections;

public class GameEntry : MonoBehaviour {

	public UILabel LobbyName;
	public UILabel Players;
	public UILabel Status;
	public UILabel PingLabel;
	public JoinServer JoinServer;
	
	Ping pinger;

	bool connected = false;
	public bool finishedConnection;
	public bool unableToConnect;
	public bool testing;

	public void GetPing(HostData host)
	{
		testing = true;
		Network.Connect(host);
		PingLabel.text = "--";
	}
	
	// Update is called once per frame
	void Update () {
		if(connected){
			int ping = Network.GetAveragePing(Network.connections[0]);
			PingLabel.text = ping.ToString();

			if(ping > 200)
			{
				PingLabel.color = Color.red;
			}

			Network.Disconnect();
			connected = false;
			testing = false;
		}
	}

	void OnConnectedToServer()
	{
		if(!testing) //avoid onconnectedtoserver calls triggering from somewhere else
			return;
		connected = true;
	}

	void OnDisconnectedFromServer(){
		if(!testing)
			return;
		finishedConnection = true;
	}

	void OnFailedToConnect(NetworkConnectionError error){
		if(!testing)
			return;
		unableToConnect = true;
		//TODO Update label to show that the game is in progress (even though the error is too many players)
	}
}

using UnityEngine;
using System.Collections;

public class MainGui : MonoBehaviour {
	
	public string connectionIP = "127.0.0.1";
	public int connectionPort = 25001;
	private const string typeName = "Distortion";
	public string gameName;
	public string playerName;
	bool wantConnection = false;
	private HostData[] hostList;
	private int lastLevelPrefix = 0;
	public UIInput playerbutton;
	public UIInput serverbutton;
	public UIButton hostbutton;
	public UIButton findbutton;




	/*

	public void OnGUI()
	{



	

		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Status: Disconnected");
			if (GUI.Button(new Rect(10, 30, 120, 20), "Refresh Hosts"))
			{
				MasterServer.RequestHostList(typeName);
				//wantConnection = true;
				//Network.Connect(connectionIP, connectionPort);
			}
			if (GUI.Button(new Rect(10, 50, 120, 20), "Initialize Server"))
			{
				Network.InitializeServer(7, connectionPort, !Network.HavePublicAddress());
				MasterServer.RegisterHost(typeName, gameName);
			}
		}
		else if (Network.peerType == NetworkPeerType.Client)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Status: Connected as Client");
			if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
			{
				Network.Disconnect(200);
				wantConnection = false;
			}
		}
		else if (Network.peerType == NetworkPeerType.Server)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Status: Connected as Server");
			if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
			{
				Network.Disconnect(200);
				wantConnection = false;
			}
			if (GUI.Button(new Rect(10, 50, 120, 20), "Play"))
			{
				Network.RemoveRPCsInGroup(0);
				Network.RemoveRPCsInGroup(1);
				//instanceManager.networkView.RPC("LoadLevel", RPCMode.AllBuffered, "Arena", lastLevelPrefix + 1);
			}
		}

		if (hostList != null)
		{
			print ("getting here");
			for (int i = 0; i < hostList.Length; i++)
			{
				if (GUI.Button(new Rect(10,90+(40*i), 120, 20), hostList[i].gameName))
					Network.Connect(hostList[i]);
			}
		}
	}
	*/
	void Update(){


		playerName = playerbutton.GetComponent<UIInput>().value;
		gameName = serverbutton.GetComponent<UIInput>().value;



	





	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
}

using UnityEngine;
using System.Collections;

public class LobbyGUI : MonoBehaviour {
	
	public string connectionIP = "127.0.0.1";
	public int connectionPort = 25001;
	private const string typeName = "Distorton";
	private const string gameName = "Test";
	bool wantConnection = false;
	private HostData[] hostList;
	private int lastLevelPrefix = 0;

	GameObject instanceManager;
	void Start(){
		instanceManager = GameObject.Find("FakeLobbySpawner");
	}
	void OnGUI()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Status: Disconnected");
			if (GUI.Button(new Rect(10, 30, 120, 20), "Client Connect"))
			{
				MasterServer.RequestHostList(typeName);
				wantConnection = true;
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
				instanceManager.networkView.RPC("LoadLevel", RPCMode.AllBuffered, "prototype", lastLevelPrefix + 1);
			}
		}
	}
	
	void Update(){
		if (hostList != null && wantConnection){
			Network.Connect(hostList[0]);
			//Should be careful here. Should really be changing this once connection is confirmed
			wantConnection = false;
		}
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
}

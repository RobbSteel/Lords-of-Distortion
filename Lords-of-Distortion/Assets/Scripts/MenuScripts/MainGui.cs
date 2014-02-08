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



//Saves player name and server name

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

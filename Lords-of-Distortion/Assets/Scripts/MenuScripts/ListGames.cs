using UnityEngine;
using System.Collections.Generic;

public class ListGames : MonoBehaviour {

	private const string typeName = "Distorton";
	private HostData[] hostList;

	List<GameObject> entries = new List<GameObject>();
	Dictionary<string, int> cachedPings = new Dictionary<string, int>();

	public UIGrid EntryGrid;
	public GameObject EntryUI;
	public AudioClip buttonhover;
	public AudioClip buttonclick;

	private float refreshtime = 0f;
	
	// Use this for initialization


	/* 
	 * Lobby Name | Joinable? | Players | Ping
	 * 
	 */


	void OnPress(bool isDown)
	{
		if (isDown) {
			audio.PlayOneShot(buttonclick);
			return;
		}
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(typeName);
		testingConnections = false;

	}
	void OnHover(bool isOver){
		if (isOver){
			audio.PlayOneShot(buttonhover, 0.35f);
		}
	}
	bool testingConnections = false;

	void GenerateList()
	{	
		foreach(GameObject entry in entries){
			NGUITools.Destroy(entry);
		}

		entries.Clear(); 

		//Takes each hosted match and adds a UI button for it upon click.
		
		for(int i = 0; i < hostList.Length; i++){
			
			//Create the buttons for servers and representative number of players
			GameObject entry = NGUITools.AddChild(EntryGrid.gameObject, EntryUI);
			entries.Add(entry);
			GameEntry gameEntry = entry.GetComponent<GameEntry>();


			//Set the buttons server value and send this value to "Join Server" script
			string ip = string.Concat(hostList[i].ip);
			if(cachedPings.ContainsKey(ip))
			{
				gameEntry.SetPing(cachedPings[ip], ip);
			}
			else 
			{
				gameEntry.RetrievePing(hostList[i]);
			}

			gameEntry.JoinServer.hostData = hostList[i];
			//check if button should be enabled
			gameEntry.JoinServer.CheckButton();

			gameEntry.Status.text = hostList[i].comment;
			/*

			if(hostList[i].connectedPlayers == 3){
				gameEntry.Status.text = "Full";
			}

			else if(hostList[i].playerLimit == 0){
				gameEntry.Status.text = "Playing";
			}

			else
			{
				gameEntry.Status.text = "Lobby";
			}
			*/
			//Set the text for the buttons that you press
			
			//UILabel server = label.GetComponentInChildren<UILabel>();
			//server.text = hostList[i].gameName;
			gameEntry.LobbyName.text = hostList[i].gameName;
			int numberofplayers = hostList[i].connectedPlayers;
			gameEntry.Players.text = numberofplayers + "/4";
		}
		EntryGrid.Reposition();

	}


	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived){

			hostList = MasterServer.PollHostList();

			if(hostList != null){
				GenerateList();
				testingConnections = true;
				currentEntry = 0;
			}
		}
	}


	int currentEntry = 0;

	// Used for constant refresh of the host list
	void Update(){
		if(refreshtime <= 0f){
			MasterServer.ClearHostList();
			MasterServer.RequestHostList(typeName);
			refreshtime = 5f;
		}

		else
		{
			refreshtime -= Time.deltaTime;
		}

		//Connect to each host to get ping, might be usefull for other things.
		if(testingConnections && hostList.Length >  0)
		{
			GameEntry gameEntry = entries[currentEntry].GetComponent<GameEntry>();
			//Skip needless testing if we have ping already.
			if(gameEntry.hasPing || gameEntry.finishedConnection || gameEntry.unableToConnect)
			{
				currentEntry++;
				//record ping
				if(!cachedPings.ContainsKey(gameEntry.ip))
					cachedPings.Add(gameEntry.ip, gameEntry.ping);
			}
			else if(!gameEntry.testing)
			{
				gameEntry.RetrievePing(hostList[currentEntry]);
			}

			//we're done
			if(currentEntry == entries.Count){
				testingConnections = false;
			}
		}
	}
}

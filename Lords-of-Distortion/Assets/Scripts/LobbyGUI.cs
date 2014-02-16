using UnityEngine;
using System.Collections;

public class LobbyGUI : MonoBehaviour {
	
	public string connectionIP = "127.0.0.1";
	public int connectionPort = 25001;
	private const string typeName = "Distorton";
	private const string gameName = "Test";
	bool wantConnection = false;
	private HostData[] hostList;
	public PSinfo infoscript;
	public Transform sessionManagerPrefab;
	SessionManager sessionManager;

	void Awake(){
		//Important
		if(!SessionManager.instanceExists){
			Transform manager = (Transform)Instantiate (sessionManagerPrefab, sessionManagerPrefab.position, Quaternion.identity);
			sessionManager = manager.GetComponent<SessionManager>();
		}
		else
			sessionManager = GameObject.FindWithTag ("SessionManager").GetComponent<SessionManager>();

	}

    public void PlayButton(GameObject go)
    {
        Network.RemoveRPCsInGroup(0);
        Network.RemoveRPCsInGroup(1);
        sessionManager.LoadNextLevel(false);
    }

    public void DisconnectButton(GameObject go)
    {
        Network.Disconnect(200);
        wantConnection = false;
    }

	void Start(){
        GameObject playButton = GameObject.Find("Play");
        UIEventListener.Get(playButton).onClick += PlayButton;

        GameObject disconnectButton = GameObject.Find("Disconnect");
        UIEventListener.Get(disconnectButton).onClick += DisconnectButton;

		var information = GameObject.Find("PSInfo");
		infoscript = information.GetComponent<PSinfo>();
		hostList = MasterServer.PollHostList();
		if(Network.peerType == NetworkPeerType.Disconnected){

			//Check if a player is hosting or joining and execute the appropriate action
			if(infoscript.choice == "Host"){
				
				Network.InitializeServer(3, connectionPort, !Network.HavePublicAddress());
				MasterServer.RegisterHost(typeName, infoscript.servername);
				
				
			} else if(infoscript.choice == "Find"){
				
				Network.Connect(hostList[infoscript.servernumb]);
				
			}
		}
	}

	void OnGUI(){
        /*if (Network.peerType == NetworkPeerType.Server)
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
                    sessionManager.LoadNextLevel(false);
                }
            }*/
        /*
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


            if (hostList != null)
            {
                //print ("getting here");
                for (int i = 0; i < hostList.Length; i++)
                {
                    if (GUI.Button(new Rect(10,90+(40*i), 120, 20), hostList[i].gameName))
                        Network.Connect(hostList[i]);
                }
            }
            */
	}

	
	void Update(){
		/*
		if (hostList != null && wantConnection){

			Network.Connect(hostList[0]);
			//Should be careful here. Should really be changing this once connection is confirmed
			wantConnection = false;
		}
		*/

	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
}

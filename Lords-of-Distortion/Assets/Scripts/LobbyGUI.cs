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

	void Start()
    {
		var information = GameObject.Find("PSInfo");
		infoscript = information.GetComponent<PSinfo>();
		hostList = MasterServer.PollHostList();
		if(Network.peerType == NetworkPeerType.Disconnected)
        {
			//Check if a player is hosting or joining and execute the appropriate action
			if(infoscript.choice == "Host")
            {
				Network.InitializeServer(3, connectionPort, !Network.HavePublicAddress());
				MasterServer.RegisterHost(typeName, infoscript.servername);
			} 
            else if(infoscript.choice == "Find")
            {
                Network.Connect(hostList[infoscript.servernumb]);
            }
		}

        if (Network.peerType == NetworkPeerType.Server)
        {
            GameObject playBtn = (GameObject)Instantiate(Resources.Load("Play"));
            GameObject disconBtn = (GameObject)Instantiate(Resources.Load("Disconnect"));

            playBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
            playBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            playBtn.transform.localPosition = new Vector2(-563.4375f, 305.625f);

            disconBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
            disconBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            disconBtn.transform.localPosition = new Vector2(-563.4375f, 230.9996f);

            UIEventListener.Get(playBtn).onClick += PlayButton;
            UIEventListener.Get(disconBtn).onClick += DisconnectButton;
        }
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
}

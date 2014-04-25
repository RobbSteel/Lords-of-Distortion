using UnityEngine;
using System.Collections;

public class LobbyGUI : MonoBehaviour {

	public int connectionPort = 25001;
	private const string typeName = "Distorton";
	private const string gameName = "Test";
	bool wantConnection = false;
	bool wantToStartGame = false;
	public PlayerServerInfo infoscript;
	public Transform sessionManagerPrefab;
	SessionManager sessionManager;

	void Awake(){
		//Important
		if(SessionManager.Instance == null){
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

		Network.maxConnections = 0;
		//Tell master server we are in game.

		MasterServer.UnregisterHost();
		//reregister host with new comment
		MasterServer.RegisterHost(typeName, infoscript.servername, "InProgress");
		sessionManager.LoadNextLevel(false);
    }

	void Update(){

	}


    public void DisconnectButton(GameObject go)
    {
        Network.Disconnect(200);
		if(Network.isServer)
			MasterServer.UnregisterHost();
        wantConnection = false;
    }

	void Start()
    {
		var information = GameObject.Find("PSInfo");
		infoscript = information.GetComponent<PlayerServerInfo>();
		if(Network.peerType == NetworkPeerType.Disconnected)
        {
			//Check if a player is hosting or joining and execute the appropriate action
			if(infoscript.choice == "Host")
            {
				Network.InitializeServer(3, connectionPort, !Network.HavePublicAddress());
				MasterServer.RegisterHost(typeName, infoscript.servername, "InLobby");
			} 
            else if(infoscript.choice == "Find")
            {
                Network.Connect(infoscript.chosenHost);
            }
		}

        if (Network.peerType == NetworkPeerType.Server)
        {
			MasterServer.updateRate = 2;
            GameObject playBtn = (GameObject)Instantiate(Resources.Load("PlayButton"));
            
            playBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
            playBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            playBtn.transform.localPosition = new Vector2(-563.4375f, -321.5379f);

            UIEventListener.Get(playBtn).onClick += PlayButton;
        }

        GameObject disconBtn = (GameObject)Instantiate(Resources.Load("Disconnect"));
        
        disconBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
        disconBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        disconBtn.transform.localPosition = new Vector2(-437.4885f, -321.5379f);

        UIEventListener.Get(disconBtn).onClick += DisconnectButton;

	}

	void OnNetworkLoadedLevel(){
		//remove all rpcs
		if(Network.isServer){

			//Tell master server that we are no longer in game.
			Network.maxConnections = 4;
			MasterServer.UnregisterHost();
			MasterServer.RegisterHost(typeName, infoscript.servername, "InLobby");
		}
	}

	//bug: this is only called on first registration
	void OnMasterServerEvent(MasterServerEvent msEvent){
		print ("did something");
		//Dont want to load level until we are sure that the host has been registered with new comment
		if(msEvent == MasterServerEvent.RegistrationSucceeded){

			if(wantToStartGame){
				sessionManager.LoadNextLevel(false);
			}
		}
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyGUI : MonoBehaviour {

	public int connectionPort = 25001;
	private const string typeName = "Distorton";
	private const string gameName = "Test";
	bool wantConnection = false;
	bool wantToStartGame = false;
    //bool playerReady = false;
	public PlayerServerInfo infoscript;
	public Transform sessionManagerPrefab;
    /*
    public GameObject readyImagePrefab;
    public List<GameObject> readyIcons;
    */
    private bool initialGridUpdate = false;

    SessionManager sessionManager;
    Dictionary<NetworkPlayer, GameObject> entries = new Dictionary<NetworkPlayer, GameObject>();

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
    /*
	void Update(){
        if (!initialGridUpdate)
        {
            for (int i = 1; i < infoscript.playernumb; i++ )
            {
                readyIcons[i].GetComponent<UISprite>().enabled = true;
            }
            initialGridUpdate = true;
        }
	}

    void OnPlayerConnected(NetworkPlayer player)
    {
        infoscript = GameObject.Find("PSInfo").GetComponent<PlayerServerInfo>();
        foreach (NetworkPlayer p in infoscript.players)
        {
            readyIcons[infoscript.playernumb].GetComponent<UISprite>().enabled = true;
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
    }
    */
    /*
    [RPC]
    void SendReadyStatus(int ready)
    {
        if(ready == 0)
        {
            readyIcons[infoscript.playernumb].GetComponent<UISprite>().color = Color.green;
        }
        else
        {
            readyIcons[infoscript.playernumb].GetComponent<UISprite>().color = Color.red;
        }
    }*/
    /*
    public void ReadyButton(GameObject go)
    {
        if(!playerReady)
        { 
            readyIcons[infoscript.playernumb].GetComponent<UISprite>().color = Color.green;
            playerReady = true;
            networkView.RPC("SendReadyStatus", RPCMode.Others, 0);
        }
        else
        {
            readyIcons[infoscript.playernumb].GetComponent<UISprite>().color = Color.red;
            networkView.RPC("SendReadyStatus", RPCMode.Others, 1);
            playerReady = false;
        }
    }
    */
    public void DisconnectButton(GameObject go)
    {
        Network.Disconnect(200);
		if(Network.isServer)
			MasterServer.UnregisterHost();
        wantConnection = false;
    }

	void Start()
    {
        infoscript = GameObject.Find("PSInfo").GetComponent<PlayerServerInfo>();
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
            playBtn.transform.localPosition = new Vector2(-545f, -321f);

            UIEventListener.Get(playBtn).onClick += PlayButton;

            //readyIcons[0].GetComponent<UISprite>().enabled = true;
        }

        GameObject disconBtn = (GameObject)Instantiate(Resources.Load("Disconnect"));
        
        disconBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
        disconBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        disconBtn.transform.localPosition = new Vector2(557.3703f, -321.5379f);

        UIEventListener.Get(disconBtn).onClick += DisconnectButton;
        /*
        GameObject readyBtn = (GameObject)Instantiate(Resources.Load("Ready"));

        readyBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
        readyBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        readyBtn.transform.localPosition = new Vector3(392.9831f, -321.5379f);

        UIEventListener.Get(readyBtn).onClick += ReadyButton;
        */
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

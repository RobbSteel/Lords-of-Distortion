using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyGUI : MonoBehaviour {

	public int connectionPort = 25001;
	private const string typeName = "Distorton";
	private const string gameName = "Test";
	bool wantConnection = false;
	bool wantToStartGame = false;
    bool playerReady = false;
	public PlayerServerInfo infoscript;
	public Transform sessionManagerPrefab;
    private int localPlayerNum;

    public UIGrid IconsGrid;

    public GameObject readyImagePrefab;
    public List<GameObject> readyIcons = new List<GameObject>();
    public Dictionary<NetworkPlayer, GameObject> dictionary = new Dictionary<NetworkPlayer,GameObject>();
    
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

    public void SetLocalPlayerNum(int num)
    {
        localPlayerNum = num;
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
    
    [RPC]
    void SendReadyStatus(int ready, NetworkPlayer player)
    {
        GameObject myLight;
        dictionary.TryGetValue(player, out myLight);

        if(ready == 0)
        {
            myLight.GetComponent<UISprite>().color = Color.green;
        }
        else
        {
            myLight.GetComponent<UISprite>().color = Color.red;
        }
    }
    
    public void ReadyButton(GameObject go)
    {
        GameObject myLight;
        dictionary.TryGetValue(Network.player, out myLight);

        if(!playerReady)
        { 
            myLight.GetComponent<UISprite>().color = Color.green;
            playerReady = true;
            networkView.RPC("SendReadyStatus", RPCMode.OthersBuffered, 0, Network.player);
        }
        else
        {
            myLight.GetComponent<UISprite>().color = Color.red;
            networkView.RPC("SendReadyStatus", RPCMode.OthersBuffered, 1, Network.player);
            playerReady = false;
        }
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
        
        GameObject readyBtn = (GameObject)Instantiate(Resources.Load("Ready"));

        readyBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
        readyBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        readyBtn.transform.localPosition = new Vector3(392.9831f, -321.5379f);

        UIEventListener.Get(readyBtn).onClick += ReadyButton;
        
	}

    public void CreateReadyLight(NetworkPlayer player)
    {
        GameObject icons = NGUITools.AddChild(IconsGrid.gameObject, readyImagePrefab);
        readyIcons.Add(icons);
        IconsGrid.Reposition();
        dictionary.Add(player, icons);
    }

    public void RemoveReadyLight(NetworkPlayer player)
    {
        GameObject myLight;
        dictionary.TryGetValue(player, out myLight);

        readyIcons.Remove(myLight);
        Destroy(myLight);
        dictionary.Remove(player);
        IconsGrid.Reposition();
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyGUI : MonoBehaviour {
	public static bool inLobby;
	//public int connectionPort = 23466;
	public int connectionPort = 25001;
	private const string typeName = "Distorton";
	private const string gameName = "Test";
	bool wantConnection = false;
	bool wantToStartGame = false;
    bool playerReady = false;
	public PlayerServerInfo infoscript;
	public Transform sessionManagerPrefab;
    private int localPlayerNum;
    private float timer = 5;
    private HUDTools hudTools;
    public UIGrid IconsGrid;
    public GameObject playbtn;

    private int connectedPlayers;
    private int numReady = 0;

    public GameObject readyImagePrefab;
    public List<GameObject> readyIcons = new List<GameObject>();
    public Dictionary<NetworkPlayer, GameObject> dictionary = new Dictionary<NetworkPlayer,GameObject>();
    
    private bool initialGridUpdate = false;

    SessionManager sessionManager;
    Dictionary<NetworkPlayer, GameObject> entries = new Dictionary<NetworkPlayer, GameObject>();

	void Awake(){
		inLobby = true;
		//MasterServer.ipAddress = "38.104.224.202";
		//Important
        playbtn = GameObject.Find("playBtn");
        hudTools = GetComponent<HUDTools>();
		if(SessionManager.Instance == null){
			Transform manager = (Transform)Instantiate (sessionManagerPrefab, sessionManagerPrefab.position, Quaternion.identity);
			sessionManager = manager.GetComponent<SessionManager>();
		}
		else
			sessionManager = SessionManager.Instance;
    }
	

    public void SetLocalPlayerNum(int num)
    {
        localPlayerNum = num;
    }

    public void DisplayMessage()
    {
        hudTools.DisplayText("Waiting for host to start...");
    }

    void Update()
    {
        if (Network.peerType == NetworkPeerType.Server)
        {
            connectedPlayers = infoscript.players.Count;
            //Debug.Log("NUM CONNECTED PLAYERS: " + connectedPlayers);
            if(numReady == connectedPlayers)
            {
                PlayButton(playbtn);
            }
        }

        if(timer < 0)
        {
            DisplayMessage();
            timer = 30f;
        }

        timer -= Time.deltaTime;
    }

	public void PlayButton(GameObject go)
    {

		Network.RemoveRPCsInGroup(0);
        Network.RemoveRPCsInGroup(1);

		Network.maxConnections = 0;
		//Tell master server we are in game.

		MasterServer.UnregisterHost();
		//reregister host with new comment
		MasterServer.RegisterHost(typeName, infoscript.servername, "Playing");
		sessionManager.LoadNextLevel(false);
    }
    
    [RPC]
    void SendReadyStatus(int ready, NetworkPlayer player)
    {
        GameObject myLight;
        dictionary.TryGetValue(player, out myLight);

        if(ready == 0)
        {
            AddNumReady();
            myLight.GetComponent<UISprite>().color = Color.green;
        }
        else
        {
            RemoveNumReady();
            myLight.GetComponent<UISprite>().color = Color.red;
        }
    }

    public void AddNumReady()
    {
        numReady += 1;
    }

    public void RemoveNumReady()
    {
        numReady -= 1;
    }

    public void ReadyButton(GameObject go)
    {
        GameObject myLight;
        dictionary.TryGetValue(Network.player, out myLight);

        if(!playerReady)
        { 
            myLight.GetComponent<UISprite>().color = Color.green;
            playerReady = true;
            AddNumReady();
            networkView.RPC("SendReadyStatus", RPCMode.OthersBuffered, 0, Network.player);
        }
        else
        {
            myLight.GetComponent<UISprite>().color = Color.red;
            RemoveNumReady();
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
		infoscript = PlayerServerInfo.Instance;
		if(Network.peerType == NetworkPeerType.Disconnected)
        {
			//Check if a player is hosting or joining and execute the appropriate action
			if(infoscript.choice == "Host")
            {
				Network.InitializeServer(20, connectionPort, !Network.HavePublicAddress());
				MasterServer.RegisterHost(typeName, infoscript.servername, "Lobby");
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

        //TODO: Change to NGUITOOLS.ADDCHILD
        GameObject disconBtn = (GameObject)Instantiate(Resources.Load("Disconnect"));
        
        disconBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
        disconBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        disconBtn.transform.localPosition = new Vector2(557.3703f, -321.5379f);

        UIEventListener.Get(disconBtn).onClick += DisconnectButton;
        
        //TODO: Change to NGUITOOLS.ADDCHILD
        GameObject readyBtn = (GameObject)Instantiate(Resources.Load("Ready"));

        readyBtn.transform.parent = GameObject.Find("UI Root LobbyArena").transform;
        readyBtn.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        readyBtn.transform.localPosition = new Vector3(392.9831f, -321.5379f);

        UIEventListener.Get(readyBtn).onClick += ReadyButton;
        
		GameInput.instance.UseCustomCursor();
	}

    public void CreateReadyLight(NetworkPlayer player)
    {
        GameObject icons = NGUITools.AddChild(IconsGrid.gameObject, readyImagePrefab);

        string playerName = infoscript.GetPlayerOptions(player).username;
        icons.GetComponentInChildren<UILabel>().text = playerName;
        icons.GetComponentInChildren<UILabel>().transform.localEulerAngles = new Vector3(0, 0, 6);

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
			Network.maxConnections = 20;
			MasterServer.UnregisterHost();
			MasterServer.RegisterHost(typeName, infoscript.servername, "Lobby");
		}
	}

	//bug: this is only called on first registration
	void OnMasterServerEvent(MasterServerEvent msEvent){
		//Dont want to load level until we are sure that the host has been registered with new comment
		if(msEvent == MasterServerEvent.RegistrationSucceeded){

			if(wantToStartGame){
				sessionManager.LoadNextLevel(false);
			}
		}
	}

	void OnDestroy()
	{
		inLobby = false;
	}

}

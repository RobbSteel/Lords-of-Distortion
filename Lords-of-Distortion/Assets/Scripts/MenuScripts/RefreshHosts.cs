using UnityEngine;
using System.Collections;

public class RefreshHosts : MonoBehaviour {

	public GameObject serverlabel;
	public GameObject playerslabel;
	private const string typeName = "Distorton";
	private HostData[] hostList;

	private float refreshtime = 0;
	// Use this for initialization
	void Start () {
	}


	void OnClick(){
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(typeName);

		if(hostList != null){


			var oldlabels = GameObject.FindGameObjectsWithTag("ServerLabel");

			//Refreshes labels

			for(int v = 0; v < oldlabels.Length;v++){


				Destroy(oldlabels[v]);

			}

			//Takes each hosted match and adds a UI button for it upon click.

			for(int i = 0; i < hostList.Length; i++){

				//Create the buttons for servers and representative number of players
				var label = (GameObject)Instantiate(serverlabel, new Vector2(0, 0), transform.rotation);
				var playernumber = (GameObject)Instantiate(playerslabel, new Vector2(0, 0), transform.rotation);
			
				/*Make each button a child of the scrolling hostlist so that it will
				be able window clip*/
				label.transform.parent = GameObject.Find("HostList").transform;
				label.transform.localScale = new Vector3(1,1,1);
				label.transform.localPosition = new Vector2(-100, 200- (50 * i));
			
				playernumber.transform.parent = GameObject.Find("HostList").transform;
				playernumber.transform.localScale = new Vector3(1,1,1);
				playernumber.transform.localPosition = new Vector2(100, 200- (50 * i));
			
				//Set the buttons server value and send this value to "Join Server" script

				var transfer = label.GetComponent<JoinServer>();
				transfer.servernumber = i;
				transfer.hostList = hostList;
			
				//Set the text for the buttons that you press

				UILabel server = label.GetComponentInChildren<UILabel>();
				server.text = hostList[i].gameName;
			
				int numberofplayers = hostList[i].connectedPlayers;
				UILabel numbofplayers = playernumber.GetComponentInChildren<UILabel>();
				numbofplayers.text = numberofplayers + " / 4";
			
				server.depth = 1;
				numbofplayers.depth = 1;
			
			}
		}
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}



	// Used for constant refresh of the host list
	void Update(){


		if(refreshtime < 0){
			MasterServer.ClearHostList();
			MasterServer.RequestHostList(typeName);
		
		if(hostList != null){
			
			
			var oldlabels = GameObject.FindGameObjectsWithTag("ServerLabel");
			
			//Refreshes labels
			
			for(int v = 0; v < oldlabels.Length;v++){
				
				
				Destroy(oldlabels[v]);
				
			}
			
			//Takes each hosted match and adds a UI button for it upon click.
			
			for(int i = 0; i < hostList.Length; i++){
				
				//Create the buttons for servers and representative number of players
				var label = (GameObject)Instantiate(serverlabel, new Vector2(0, 0), transform.rotation);
				var playernumber = (GameObject)Instantiate(playerslabel, new Vector2(0, 0), transform.rotation);
				
				/*Make each button a child of the scrolling hostlist so that it will
				be able window clip*/
				label.transform.parent = GameObject.Find("HostList").transform;
				label.transform.localScale = new Vector3(1,1,1);
				label.transform.localPosition = new Vector2(-100, 200- (50 * i));
				
				playernumber.transform.parent = GameObject.Find("HostList").transform;
				playernumber.transform.localScale = new Vector3(1,1,1);
				playernumber.transform.localPosition = new Vector2(100, 200- (50 * i));
				
				//Set the buttons server value and send this value to "Join Server" script
				
				var transfer = label.GetComponent<JoinServer>();
				transfer.servernumber = i;
				transfer.hostList = hostList;
				
				//Set the text for the buttons that you press
				
				UILabel server = label.GetComponentInChildren<UILabel>();
				server.text = hostList[i].gameName;
				
				int numberofplayers = hostList[i].connectedPlayers;
				UILabel numbofplayers = playernumber.GetComponentInChildren<UILabel>();
				numbofplayers.text = numberofplayers + " / 4";
				
				server.depth = 1;
				numbofplayers.depth = 1;
				refreshtime = 3;
			}
		}
	} else {

			refreshtime -= Time.deltaTime;
		}
}
}

using UnityEngine;
using System.Collections;

public class RefreshHosts : MonoBehaviour {

	public GameObject serverlabel;
	private const string typeName = "Distorton";
	private HostData[] hostList;
	// Use this for initialization
	void Start () {
	
	}


	void OnClick(){

		MasterServer.RequestHostList(typeName);

		if(hostList != null){

		for(int i = 0; i < hostList.Length; i++){

			var label = (GameObject)Instantiate(serverlabel, new Vector2(0, 0), transform.rotation);
			label.transform.parent = GameObject.Find("HostList").transform;
			label.transform.localScale = new Vector3(1,1,1);
			label.transform.localPosition = new Vector2(-100, 200- (50 * i));
			var transfer = label.GetComponent<JoinServer>();
			transfer.servernumber = i;
			transfer.hostList = hostList;
			UILabel server = label.GetComponentInChildren<UILabel>();
			
			server.text = hostList[i].gameName;
			
			server.depth = 1;
			
		}
	}
}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}



	// Update is called once per frame
	void Update () {
	
	}
}

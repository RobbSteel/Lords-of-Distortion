using UnityEngine;
using System.Collections;

public class JoinServer : MonoBehaviour {

	public int servernumber;
	public HostData[] hostList;
	// Use this for initialization
	void Start () {
	
	}

	void OnClick(){

		print (hostList);
		Network.Connect(hostList[servernumber]);

	}

	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class LeaveGame : MonoBehaviour {

    void OnPress()
    {
        Network.Disconnect(200);
		if(Network.isServer)
			MasterServer.UnregisterHost();
    }

}

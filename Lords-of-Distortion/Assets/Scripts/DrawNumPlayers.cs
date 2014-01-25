using UnityEngine;
using System.Collections;

public class DrawNumPlayers : MonoBehaviour {

    private GameObject p1;
    private GameObject p2;
    private GameObject p3;
    private GameObject p4;
    private GameObject lobbyInstncMngr;

    private int player1Num;
    private int player2Num;
    private int player3Num;
    private int player4Num;

    // Update is called once per frame
    void Start()
    {
        p1 = GameObject.Find("P1");
        p2 = GameObject.Find("P2");
        p3 = GameObject.Find("P3");
        p4 = GameObject.Find("P4");
        //lobbyInstncMngr = GameObject.Find("FakeLobbySpawner");

        //player1Num = lobbyInstncMngr.GetComponent<LobbyInstanceManager>().;
    }
    
    void Update()
    {
        if(Network.connections.Length > 0)
        {
            p2.gameObject.GetComponent<UISprite>().enabled = true;
        }
        if (Network.connections.Length > 1)
        {
            p3.gameObject.GetComponent<UISprite>().enabled = true;
        }
        if (Network.connections.Length > 2)
        {
            p4.gameObject.GetComponent<UISprite>().enabled = true;
        }
    }
}

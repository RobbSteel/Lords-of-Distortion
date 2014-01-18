using UnityEngine;
using System.Collections;

public class GravityField : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        GravField();
	}

    public void GravField()
    {
        
        if (Input.GetKeyDown(KeyCode.G) == true)
        {
            // Local Spawn
            GameObject gravField = (GameObject)Instantiate(Resources.Load("gravityField"));
            Destroy(gravField, 5f);
            
            // Network Spawn - Not sure if this works, fails because "We are not connected"
            //GameObject gravField = (GameObject)Network.Instantiate(Resources.Load("gravityField"), new Vector3(0, 0, 0), Quaternion.identity, -1);
        }
    }

    void OnDestroy()
    {
        Debug.Log("Destroyed");
        GameObject user = GameObject.FindGameObjectWithTag("Player");
        user.rigidbody2D.gravityScale = 1;
    }

}

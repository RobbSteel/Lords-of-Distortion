using UnityEngine;
using System.Collections;

public class GravityField : MonoBehaviour {

    private GameObject gravField;
    public float timer = 0;

	// Update is called once per frame
	void Update () {
        GravField();
	}

    public void GravField()
    {
        if (Input.GetKeyDown(KeyCode.G) == true)
        {
            // Local Spawn
            //GameObject gravField = (GameObject)Instantiate(Resources.Load("gravityField"));

            timer = Time.time;

            // Network Spawn - Not sure if this works, fails because "We are not connected"
            gravField = (GameObject)Network.Instantiate(Resources.Load("gravityField"), new Vector3(0, 0, 0), Quaternion.identity, -1);
        }

        if (Time.time - timer > 5)
            Network.Destroy(gravField);
    }

    void OnDestroy()
    {
        Debug.Log("Destroyed");
        GameObject user = GameObject.FindGameObjectWithTag("Player");
        if (user != null)
            user.rigidbody2D.gravityScale = 1;
    }

}

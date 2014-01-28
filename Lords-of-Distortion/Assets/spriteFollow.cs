using UnityEngine;
using System.Collections;

public class spriteFollow : MonoBehaviour {

    public GameObject objectToInstantiate;

    private bool btnClicked = false;

    private Camera camera;
    private GameObject myCurrentObject;
   

	// Use this for initialization
	void Start ()
    {
	    camera = Camera.main;
	}
	
    void OnClick()
    {
        btnClicked = true;
    }

	// Update is called once per frame
	void Update ()
    {
        //OnPressed(clicked);
        if (btnClicked)
        {
            myCurrentObject = (GameObject)Instantiate(objectToInstantiate, camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f)), Quaternion.identity);
            btnClicked = false;
            Vector3 temp = new Vector3(0, 0, 10.0f);
            myCurrentObject.transform.position += temp;
        }
        
        if (myCurrentObject != null)
        {
            myCurrentObject.transform.position = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
            Vector3 temp = new Vector3(0, 0, 10.0f);
            myCurrentObject.transform.position += temp;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Destroy(myCurrentObject);
            //myCurrentObject = null;
        }
    }

}

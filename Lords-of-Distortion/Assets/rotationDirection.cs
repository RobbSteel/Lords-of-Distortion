using UnityEngine;
using System.Collections;

public class rotationDirection : MonoBehaviour {

    public Material mat;

    private int clicks = 0;
    private int lrLength = 2;

    private bool lrCreated = false;

    private Vector3 firstClick;
    private Vector3 aimTowards;

    private LineRenderer lineRenderer;

	// Use this for initialization
	void Awake () {
        /*lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetWidth(.1f, .1f);
        lineRenderer.material = mat;
        lineRenderer.sortingLayerName = "Player";
        lineRenderer.sortingOrder = 1;*/
	}
	
	// Update is called once per frame
	void Update () {

        /*if(Input.GetMouseButtonUp(0) && clicks == 0)
        {
            firstClick = Input.mousePosition;
            firstClick.z = 10.0f;
            clicks++;
        }
        if(clicks == 1)
        {
            if(!lrCreated)
            { 
                lineRenderer.SetPosition(0, firstClick);
                lrCreated = true;
            }
            if(lrCreated)
            { 
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10.0f;
                lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(mousePos));
            }
        }
        if(Input.GetMouseButtonUp(0) && clicks == 1)
        {
            LineRenderer.Destroy(lineRenderer);
            lrCreated = false;
            clicks = 0;
        }*/
        /*
        var mousePos = Input.mousePosition;
        mousePos.z = 10.0f; //The distance from the camera to the player object
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        /*Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);*/
	}
}

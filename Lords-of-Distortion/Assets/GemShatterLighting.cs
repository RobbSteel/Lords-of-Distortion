using UnityEngine;
using System.Collections;

public class GemShatterLighting : MonoBehaviour {

    public Light light;

    private bool shattered = false;
    private float timer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
	    if(shattered && timer > .25)
        {
            light.intensity += 0.46875f;
            timer = 0;
        }
	}

    public void GemShattered()
    {
        shattered = true;
    }


}

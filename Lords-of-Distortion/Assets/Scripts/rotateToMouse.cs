using UnityEngine;
using System.Collections;

public class rotateToMouse : MonoBehaviour
{
    bool aiming = true;
	public float speed = 0;
    private GameObject fireball;

	void Awake(){
		aiming = true;
	}
	
	
	// Use this for initialization
	void Start ()
	{
	}
	
	
	
	// Update is called once per frame
	void Update (){
        Debug.Log("Update Function Rotate To Mosue");
			/*float dist = Vector3.Distance(new Vector3(0,0,0), transform.position);
			if (dist >= 7.0f) {
						//Destroy (this.gameObject);
				
			
			*/
        if(!aiming)
            transform.Translate(speed, 0, 0);
			
        if(aiming) {
                Debug.Log("Aiming Fireball");
				var mousePos = Input.mousePosition;
                
				mousePos.z = 10.0f; //The distance from the camera to the player object
                Debug.Log(mousePos);
                Vector3 lookPos = Camera.main.ScreenToWorldPoint (mousePos);
				lookPos = lookPos - transform.position;
				float angle = Mathf.Atan2 (lookPos.y, lookPos.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
				
				//transform.LookAt(Camera.main.ScreenToWorldPoint (mousePos));
				
			}
			
			if (Input.GetMouseButtonUp(0) == true && aiming == true) {
				aiming = false;
				speed = .1f;
                Debug.Log("No Longer Aiming Fireball");
                fireball = (GameObject)Instantiate(Resources.Load("fireball"));	
			}
	}

}
using UnityEngine;
using System.Collections;

public class GravityFieldIndicators : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject indicatorA = new GameObject("GField_PointA");
        GameObject indicatorB = new GameObject("GField_PointB");
        Sprite arrowSprite = Resources.Load("arrow", typeof(Sprite)) as Sprite;

        indicatorA.transform.parent = transform;
        indicatorA.AddComponent<SpriteRenderer>();
        indicatorA.GetComponent<SpriteRenderer>().sprite = arrowSprite;
        indicatorA.transform.localPosition = new Vector3(1.5f, 0, 0);
        indicatorA.layer = 9;

        indicatorB.transform.parent = transform;
        indicatorB.AddComponent<SpriteRenderer>();
        indicatorB.GetComponent<SpriteRenderer>().sprite = arrowSprite;
        indicatorB.transform.localPosition = new Vector3(-3, 0, 0);
        indicatorB.transform.localScale = new Vector3(-1, 1, 1);
        indicatorB.layer = 9;
	}
	
}

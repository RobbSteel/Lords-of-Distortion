using UnityEngine;
using System.Collections;

public class BirdCageFade : MonoBehaviour {


    public float durationOffFade;
	
	private SpriteRenderer cageSprite;
	private Color transparent = Color.clear;
	private Color nonTransparent = Color.white;
	private string playerTag = "Player";
	private bool entered;
	private float fadingTimer;

	// Use this for initialization
	void Start () {
		cageSprite = this.GetComponent<SpriteRenderer>();
		entered = false;
	}

	//checks if player has entered cage and starts correct fade
	void OnTriggerEnter2D( Collider2D col ){
		if( col.CompareTag( playerTag ) ){
			StopCoroutine("NonTransparentCage");
			StartCoroutine("TransparentCage");
		}
	}

	void OnTriggerStay2D( Collider2D col ){

	}

	//checks if player has exit cage and starts correct fade
	void OnTriggerExit2D( Collider2D col ){
		if( col.CompareTag( playerTag ) ){
			StopCoroutine("TransparentCage");
			StartCoroutine("NonTransparentCage");
		}
	}

	//fades sprite nonVisible
	IEnumerator TransparentCage(){
		fadingTimer = 0;
		Color current = cageSprite.color;
		while (Vector4.Distance( transparent , cageSprite.color) > 0f ){
			cageSprite.color = Vector4.Lerp (current, transparent , fadingTimer / durationOffFade);
			fadingTimer += Time.deltaTime;
			yield return new WaitForEndOfFrame();

		}

		yield return null;
	}

	//fades sprite visible
	IEnumerator NonTransparentCage(){
		fadingTimer = 0;
		Color current = cageSprite.color;
		while (Vector4.Distance( nonTransparent , cageSprite.color) > 0f ){
			cageSprite.color = Vector4.Lerp (current, nonTransparent , fadingTimer / durationOffFade );
			fadingTimer += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		yield return null;

	}
}

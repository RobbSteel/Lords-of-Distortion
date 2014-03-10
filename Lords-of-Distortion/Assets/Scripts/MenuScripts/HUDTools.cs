using UnityEngine;
using System.Collections;

public class HUDTools : MonoBehaviour {
	public UILabel TextPrefab;


	// Use this for initialization
	void Start () {
		//DisplayText("Yo this is some text", 3f);
	}


	IEnumerator FadeInOut(UILabel label, float time) {
		NGUITools.AddChild(label.gameObject);
		TweenAlpha tween = label.GetComponent<TweenAlpha>();
		tween.PlayForward();
		yield return new WaitForSeconds(time);
		tween.PlayReverse();
		EventDelegate.Add(tween.onFinished, Destroy);
	}

	void Destroy(){
		//Get the tweener that called this function and destroy it.
		NGUITools.Destroy(UITweener.current.gameObject);
	}

	/// <summary>
	/// Displays Text for a certain amount of time on the scenes UI camera.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="displayTime">Display time.</param>
	public void DisplayText(string text, float displayTime = 1.8f){
		UILabel label = (UILabel)Instantiate(TextPrefab, TextPrefab.transform.position, TextPrefab.transform.rotation);
		label.text = text;
		StartCoroutine(FadeInOut(label, displayTime));
	}
}

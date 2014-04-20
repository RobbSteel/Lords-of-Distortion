using UnityEngine;
using System.Collections;

public class HUDTools : MonoBehaviour {
	public UILabel TextPrefab;
	public GameObject heartParticles;

	public UILabel PointTextPrefab;

	public UIFont scoreFont;

	// Use this for initialization
	void Start () {
		//DisplayText("Yo this is some text", 3f);
	}


	IEnumerator FadeInOut(UILabel label, float time) {
		//NGUITools.AddChild(label.gameObject);
		TweenAlpha tween = label.GetComponent<TweenAlpha>();
		tween.PlayForward();
		yield return new WaitForSeconds(time);
		tween.PlayReverse();
		EventDelegate.Add(tween.onFinished, DestroyLabel);
	}

	void DestroyLabel(){
		//Get the tweener that called this function and destroy it.
		NGUITools.Destroy(UITweener.current.gameObject);
	}

	/// <summary>
	/// Displays Text for a certain amount of time on the scene's UI camera.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="displayTime">Display time.</param>
	public void DisplayText(string text, float displayTime = 1.8f){
		UILabel label = (UILabel)Instantiate(TextPrefab, TextPrefab.transform.position, TextPrefab.transform.rotation);
		label.text = text;
		StartCoroutine(FadeInOut(label, displayTime));
	}
	

	public void ShowPoints(float points, GameObject player)
	{
		float displayTime = 1.0f;
	
		UILabel label = (UILabel)Instantiate(PointTextPrefab, Vector3.zero, PointTextPrefab.transform.rotation);

		label.text = "+" + points.ToString();

	
		UIFollowTarget followTarget = label.GetComponent<UIFollowTarget>();
		followTarget.Target = player.transform;
			
		GameObject hearts = Instantiate(heartParticles, player.transform.position, Quaternion.identity) as GameObject;
		hearts.transform.parent = player.transform;
		hearts.transform.localPosition = new Vector3(0f, player.GetComponent<BoxCollider2D>().size.y / 2, 0);


		//TODO: if a player already is showing points, put above those.
		followTarget.offset.y = player.renderer.bounds.size.y / 2f;

		StartCoroutine(FadeInOut(label, displayTime));
	}
}

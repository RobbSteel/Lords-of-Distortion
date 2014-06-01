using UnityEngine;
using System.Collections;

public class FlipLevel : MonoBehaviour {
	
	[SerializeField]
	private string LevelName = "MainMenu";
	public bool flipleft = false;
	public GameObject transitioner;
	
	
	Transitioner fader;
	
	
	
	
	
	void Awake(){
		
		fader = transitioner.GetComponent<Transitioner>();
	}
	
	void OnPress(bool isDown)
	{
		if(!isDown)
		{
			fader.Flip(LevelName, flipleft);
		}
	}
}
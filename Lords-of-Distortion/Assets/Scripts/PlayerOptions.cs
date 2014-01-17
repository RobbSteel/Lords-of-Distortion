using UnityEngine;
using System.Collections;

public class PlayerOptions{
	public enum CharacterStyle
	{
		DEFAULT, RED, BLUE
	}

	public int playerNumber;
	public string username;
	public CharacterStyle style;

	public PlayerOptions(){
		style = CharacterStyle.DEFAULT;
	}
}

using UnityEngine;
using System.Collections;

public class PlayerOptions{
	public enum CharacterStyle
	{
		BLUE = 0, RED, GREEN, YELLOW, DEFAULT
	}

	public CharacterStyle style;
	public string username;

	private int playerNumber;

	public int PlayerNumber{
		get{ return playerNumber;}
		set{
			playerNumber = value;
			style = (CharacterStyle)value;
		}
	}

	public PlayerOptions(){
		style = CharacterStyle.DEFAULT;
	}

}

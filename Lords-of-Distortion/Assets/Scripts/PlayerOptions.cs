using UnityEngine;
using System.Collections;

public class PlayerOptions{

	public enum CharacterStyle
	{
		DEFAULT = 0, RED, GREEN, BLUE, YELLOW
	}

	public enum Character{
		Colossus = 0, Blue
	}

	public CharacterStyle style;
	public string username;
    public Character character;

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

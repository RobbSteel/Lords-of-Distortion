using UnityEngine;
using System.Collections;


public enum CharacterStyle
{
	DEFAULT = 0, RED, GREEN, BLUE, YELLOW
}

public enum Character{
	Colossus = 0, Blue , Mummy
}

public class PlayerOptions{



	public CharacterStyle style;
	public string username;
    public Character character = Character.Colossus;

	private int playerNumber;
	
	public int PlayerNumber{
		get{ return playerNumber;}
		set{
			playerNumber = value;
		}
	}

	public PlayerOptions(){
		style = CharacterStyle.DEFAULT;
	}
}

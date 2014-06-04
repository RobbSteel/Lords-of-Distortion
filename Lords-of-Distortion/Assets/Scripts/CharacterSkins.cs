using UnityEngine;
using System.Collections.Generic;


public class CharacterSkins : MonoBehaviour 
{

	[SerializeField]
	private GameObject ColossusPrefab;
	[SerializeField]
	private GameObject BluePrefab;
	[SerializeField]
	private GameObject MummyPrefab;

	[SerializeField]
	private RuntimeAnimatorController C_GreenAnimator;
	[SerializeField]
	private RuntimeAnimatorController C_YellowAnimator;
	[SerializeField]
	private RuntimeAnimatorController B_BlueAnimator;
	[SerializeField]
	private RuntimeAnimatorController M_RedAnimator;



	private Dictionary<CharacterAndStyle, RuntimeAnimatorController> animators = new Dictionary<CharacterAndStyle, RuntimeAnimatorController>();

	void Awake()
	{
		CharacterAndStyle C_Green = new CharacterAndStyle(Character.Colossus, CharacterStyle.GREEN);
		animators.Add(C_Green, C_GreenAnimator);
		CharacterAndStyle C_Yellow = new CharacterAndStyle(Character.Colossus, CharacterStyle.YELLOW);
		animators.Add(C_Yellow, C_YellowAnimator);

	}

	//Selects appropriate prefab and animation controller for character.
	//Returns an inavtive object by default.
	public GameObject GenerateRecolor(Character character, CharacterStyle color)
	{
		GameObject copy;

		CharacterAndStyle option = new CharacterAndStyle(character, color);

		switch(character){
			case Character.Colossus:
			copy = Instantiate(ColossusPrefab) as GameObject;
				break;
			case Character.Blue:
			copy = Instantiate(BluePrefab) as GameObject;
				break;
			case Character.Mummy:
			copy = Instantiate(MummyPrefab) as GameObject;
				break;
			default:
			copy = Instantiate(ColossusPrefab) as GameObject;
				break;
		}

		RuntimeAnimatorController controller = animators[option];
		copy.GetComponent<Animator>().runtimeAnimatorController = controller;

		return copy;
	}
}
//for use in dictionary
public struct CharacterAndStyle{
	public  Character character;
	public  CharacterStyle style;
	public CharacterAndStyle(Character character, CharacterStyle style)
	{
		this.character = character;
		this.style = style;
	}
}


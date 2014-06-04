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
	private RuntimeAnimatorController C_BlueAnimator;
	[SerializeField]
	private RuntimeAnimatorController C_GreenAnimator;
	[SerializeField]
	private RuntimeAnimatorController C_RedAnimator;
	[SerializeField]
	private RuntimeAnimatorController C_YellowAnimator;
	[SerializeField]
	private RuntimeAnimatorController B_BlueAnimator;
	[SerializeField]
	private RuntimeAnimatorController B_GreenAnimator;
	[SerializeField]
	private RuntimeAnimatorController B_RedAnimator;
	[SerializeField]
	private RuntimeAnimatorController B_YellowAnimator;
	[SerializeField]
	private RuntimeAnimatorController M_RedAnimator;
	[SerializeField]
	private RuntimeAnimatorController M_GreenAnimator;



	private Dictionary<CharacterAndStyle, RuntimeAnimatorController> animators = new Dictionary<CharacterAndStyle, RuntimeAnimatorController>();

	void Awake()
	{
		//Relate combinations to animators.
		CharacterAndStyle C_Blue = new CharacterAndStyle(Character.Colossus, CharacterStyle.BLUE);
		animators.Add (C_Blue, C_BlueAnimator);

		CharacterAndStyle C_Green = new CharacterAndStyle(Character.Colossus, CharacterStyle.GREEN);
		animators.Add(C_Green, C_GreenAnimator);
		CharacterAndStyle C_Yellow = new CharacterAndStyle(Character.Colossus, CharacterStyle.YELLOW);
		animators.Add(C_Yellow, C_YellowAnimator);
		CharacterAndStyle C_Red = new CharacterAndStyle(Character.Colossus, CharacterStyle.RED);
		animators.Add(C_Red, C_RedAnimator);


		CharacterAndStyle B_Blue = new CharacterAndStyle(Character.Blue, CharacterStyle.BLUE);
		animators.Add(B_Blue, B_BlueAnimator);
		CharacterAndStyle B_Green = new CharacterAndStyle(Character.Blue, CharacterStyle.GREEN);
		animators.Add(B_Green, B_GreenAnimator);
		CharacterAndStyle B_Red = new CharacterAndStyle(Character.Blue, CharacterStyle.RED);
		animators.Add(B_Red, B_RedAnimator);
		CharacterAndStyle B_Yellow = new CharacterAndStyle(Character.Blue, CharacterStyle.YELLOW);
		animators.Add(B_Yellow, B_YellowAnimator);

		CharacterAndStyle M_Green = new CharacterAndStyle(Character.Mummy, CharacterStyle.GREEN);
		animators.Add(M_Green, M_GreenAnimator);
		CharacterAndStyle M_Red = new CharacterAndStyle(Character.Mummy, CharacterStyle.RED);
		animators.Add(M_Red, M_RedAnimator);
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
		print (character + " " + color);
		RuntimeAnimatorController controller = animators[option];
		copy.GetComponent<Animator>().runtimeAnimatorController = controller;
		copy.SetActive(false);
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


using UnityEngine;
using System.Collections.Generic;


public class CharacterSkins : MonoBehaviour 
{

	public static CharacterSkins instance = null;
	[SerializeField]
	private GameObject ColossusPrefab;
	[SerializeField]
	private GameObject BluePrefab;
	[SerializeField]
	private GameObject MummyPrefab;

	//Animation controllers
	public RuntimeAnimatorController C_BlueAnimator;
	public RuntimeAnimatorController C_GreenAnimator;
	public RuntimeAnimatorController C_RedAnimator;
	public RuntimeAnimatorController C_YellowAnimator;
	
	public RuntimeAnimatorController B_BlueAnimator;
	public RuntimeAnimatorController B_GreenAnimator;
	public RuntimeAnimatorController B_RedAnimator;
	public RuntimeAnimatorController B_YellowAnimator;
	
	public RuntimeAnimatorController M_BlueAnimator;
	public RuntimeAnimatorController M_YellowAnimator;
	public RuntimeAnimatorController M_RedAnimator;
	public RuntimeAnimatorController M_GreenAnimator;

	//HeadTextures
	public UITexture C_GreenHead;

	//Body Sprites
	public GameObject C_GreenBody;

	private Dictionary<CharacterAndStyle, RuntimeAnimatorController> animators = new Dictionary<CharacterAndStyle, RuntimeAnimatorController>();
	private Dictionary<CharacterAndStyle, UITexture> headTextures = new Dictionary<CharacterAndStyle, UITexture>();
	private Dictionary<CharacterAndStyle, GameObject> bodySprites = new Dictionary<CharacterAndStyle, GameObject>();

	void Awake()
	{

		instance = this;

		//Relate combinations to animators.
		CharacterAndStyle C_Blue = new CharacterAndStyle(Character.Colossus, CharacterStyle.BLUE);
		CharacterAndStyle C_Green = new CharacterAndStyle(Character.Colossus, CharacterStyle.GREEN);
		CharacterAndStyle C_Yellow = new CharacterAndStyle(Character.Colossus, CharacterStyle.YELLOW);
		CharacterAndStyle C_Red = new CharacterAndStyle(Character.Colossus, CharacterStyle.RED);

		animators.Add (C_Blue, C_BlueAnimator);
		animators.Add(C_Green, C_GreenAnimator);
		animators.Add(C_Yellow, C_YellowAnimator);
		animators.Add(C_Red, C_RedAnimator);

		CharacterAndStyle B_Blue = new CharacterAndStyle(Character.Blue, CharacterStyle.BLUE);
		CharacterAndStyle B_Green = new CharacterAndStyle(Character.Blue, CharacterStyle.GREEN);
		CharacterAndStyle B_Red = new CharacterAndStyle(Character.Blue, CharacterStyle.RED);
		CharacterAndStyle B_Yellow = new CharacterAndStyle(Character.Blue, CharacterStyle.YELLOW);

		animators.Add(B_Blue, B_BlueAnimator);
		animators.Add(B_Green, B_GreenAnimator);
		animators.Add(B_Red, B_RedAnimator);
		animators.Add(B_Yellow, B_YellowAnimator);

		CharacterAndStyle M_Green = new CharacterAndStyle(Character.Mummy, CharacterStyle.GREEN);
		CharacterAndStyle M_Red = new CharacterAndStyle(Character.Mummy, CharacterStyle.RED);
		CharacterAndStyle M_Blue = new CharacterAndStyle(Character.Mummy, CharacterStyle.BLUE);
		CharacterAndStyle M_Yellow = new CharacterAndStyle(Character.Mummy, CharacterStyle.YELLOW);

		animators.Add(M_Green, M_GreenAnimator);
		animators.Add(M_Red, M_RedAnimator);
		animators.Add(M_Blue, M_BlueAnimator);
		animators.Add(M_Yellow, M_YellowAnimator);


		//Example for head textures. Note keys are already available
		headTextures.Add(C_Green, C_GreenHead);

		//Example for body sprites
		bodySprites.Add(C_Green, C_GreenBody);

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

	public UITexture GetHeadTexture(Character character, CharacterStyle color)
	{
		CharacterAndStyle option = new CharacterAndStyle(character, color);
		return headTextures[option];
	}

	public GameObject GetBodyGameObjet(Character character, CharacterStyle color)
	{
		CharacterAndStyle option = new CharacterAndStyle(character, color);
		return bodySprites[option];
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


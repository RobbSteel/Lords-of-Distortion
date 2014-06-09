using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharSelectScript : MonoBehaviour {

    public List<GameObject> charList;
    public List<GameObject> textureList;
	public List<GameObject> nameList;

    // When clicking a character at character select, this handles changing the backgroud color
    public void UpdateBackgroundColor(int charNum)
    {
        for(int i = 0; i < charList.Count; i++)
        {
            if (charList[i].GetComponent<CharSelected>().characterNum == charNum)
            {
                charList[i].GetComponent<UIButton>().defaultColor = Color.red;
                textureList[i].GetComponent<UITexture>().color = Color.red;
				nameList[i].GetComponent<UITexture>().color = Color.white;
				nameList[i].GetComponent<UILabel>().color = Color.white;

            }
            else 
            {
                charList[i].GetComponent<UIButton>().defaultColor = Color.grey;
                textureList[i].GetComponent<UITexture>().color = Color.grey;
				nameList[i].GetComponent<UITexture>().color = Color.grey;
				nameList[i].GetComponent<UILabel>().color = Color.grey;
			
                charList[i].GetComponent<UIButton>().UpdateColor(true, true);
            }
        }
    }
}

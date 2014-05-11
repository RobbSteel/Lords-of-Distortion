using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharSelectScript : MonoBehaviour {

    public List<GameObject> charList;
    public List<GameObject> textureList;

    // When clicking a character at character select, this handles changing the backgroud color
    public void UpdateBackgroundColor(int charNum)
    {
        for(int i = 0; i < charList.Count; i++)
        {
            if (charList[i].GetComponent<CharSelected>().characterNum == charNum)
            {
                charList[i].GetComponent<UIButton>().defaultColor = Color.green;
                textureList[i].GetComponent<UITexture>().color = Color.green;
            }
            else 
            {
                charList[i].GetComponent<UIButton>().defaultColor = Color.white;
                textureList[i].GetComponent<UITexture>().color = Color.white;
                charList[i].GetComponent<UIButton>().UpdateColor(true, true);
            }
        }
    }
}

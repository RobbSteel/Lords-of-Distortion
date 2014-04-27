using UnityEngine;
using System.Collections;

public class CharSelected : MonoBehaviour {
	
    public PlayerServerInfo infoscript;
    public int characterNum;
    
	void OnPress(bool isDown)
    {
        if(isDown)
			return;

        infoscript.localOptions.character = (PlayerOptions.Character)characterNum;
	}	
}

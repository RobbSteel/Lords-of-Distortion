using UnityEngine;
using System.Collections;

public class MusicHandler : MonoBehaviour {

    private static MusicHandler instance = null;
    
    public static MusicHandler Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
			GetComponent<AudioSource>().enabled = true;
        }
        DontDestroyOnLoad(this.gameObject);
    }
	
}

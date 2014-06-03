using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public AudioClip newMusic;

    void Awake()
    {
        GameObject go = GameObject.Find("GameMusic");
        if(go.audio.clip != newMusic)
        { 
            go.audio.clip = newMusic;
            go.audio.Play();
        }
    }
}

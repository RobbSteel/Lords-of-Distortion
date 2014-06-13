using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public AudioClip newMusic;
    private MusicHandler handler;

    void Awake()
    {
        GameObject go = GameObject.Find("GameMusic");
        if (go!= null && go.audio.clip != newMusic)
        {
            go.audio.clip = newMusic;
            go.audio.Play();
        }
    }
}

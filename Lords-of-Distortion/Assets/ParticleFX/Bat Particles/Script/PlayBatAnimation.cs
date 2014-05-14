using UnityEngine;
using System.Collections;

public class PlayBatAnimation : MonoBehaviour {

	public GameObject[] particleArray;
	public float timeTillPlay;
	public bool random;
	private float timer;

	// Use this for initialization
	void Start () {
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		ParticlePlayManager ();
	}

	//plays a particle within a specifc position
	void PlayParticleSystem( int position ){
		particleArray[position].GetComponent<ParticleSystem>().Play();
	}

	//play a random particle within the array
	void RandomParticlePlay(){
		int arrayPostion = (int)Mathf.Round( Random.Range (0f, particleArray.Length - 1) );
		PlayParticleSystem (arrayPostion);
	}

	void AllParticlePlay(){
		for( int i = 0; i <= particleArray.Length - 1; ++i ){
			PlayParticleSystem( i );
			Debug.Log( i + ":Particle" );
		}
	}

	//main logic to decide when and what animation to play
	void ParticlePlayManager(){
		timer += Time.deltaTime;
		if (timer > timeTillPlay) {

			if( random )
			RandomParticlePlay();
			else
			AllParticlePlay();
			
			timer = 0;
		}
	}
}

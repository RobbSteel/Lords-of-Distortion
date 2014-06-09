using UnityEngine;
using System.Collections;

public class randomColorBirds : MonoBehaviour {


	public bool randomColor;
	public bool randomSolidColor;
	public float colorSpawnLength;
	private float timer;
	private ParticleSystem birdParticles;

	// Use this for initialization
	void Start () {
		timer = 0;
		birdParticles = this.GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		setBirdColor ();
	}

	Vector4 randomColorCreate(){
		return new Vector4 (randomNumber (), randomNumber (), randomNumber (), randomNumber ());
	}

	Vector4 randomSolidColorCreate(){
		return new Vector4 (randomNumber (), randomNumber (), randomNumber (), 1f );
	}

	float randomNumber(){
		return Random.Range (0f, 1f);
	}

	void setBirdColor(){
		timer += Time.deltaTime;

		if( timer >= colorSpawnLength ){
			timer = 0;

			if( randomSolidColor )
			birdParticles.startColor = randomSolidColorCreate();

			if( randomColor  )
			birdParticles.startColor = randomColorCreate();

		}
	
	}
}

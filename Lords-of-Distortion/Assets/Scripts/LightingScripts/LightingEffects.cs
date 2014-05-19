using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightingEffects : MonoBehaviour {
		
	private Light lighting;
	[Range(0, 8)] public float glowEndingTween;
	[Range(0, 8)] public float glowStartingTween;
	[Range(0, Mathf.Infinity)] public float rangeEndingTween;
	[Range(0, Mathf.Infinity)] public float rangeStartingTween;
	public bool glowTween;
	public bool rangeTween;
	public float glowTweenLength;
	public float rangeTweenLength;
	public float startTweenDelay;
	private float startDelayTimer;
	private float glowTimer;
	private float rangeTimer;
	private bool switchGlowTween;
	private bool switchRangeTween;


	// Use this for initialization
	void Start () {
		lighting = this.GetComponent<Light> ();

		//if wanted then set startingTween else leave as default
		if( glowTween )
		lighting.intensity = glowStartingTween;
		//if wanted then set startingTween else leave as default
		if( rangeTween )
		lighting.range = rangeStartingTween;
		startDelayTimer = 0;
		switchGlowTween = true;
		switchRangeTween = true;
		glowTimer = 0;
		rangeTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (startDelayTimer >= startTweenDelay) {
			GlowTweening ();
			RangeTweening ();
		}
		else
			startDelayTimer += Time.deltaTime;

	}

	//executes glowing tween for lights.
	private void GlowTweening(){
		if( glowTween ){
			//checks Timer to see if its time to switch glow tween
			if( glowTimer >= glowTweenLength ){
				glowTimer = 0;
				switchGlowTween = !switchGlowTween;
			}

			if( switchGlowTween )
				glowTweenForward();
			if( !switchGlowTween )
				glowTweenBackward();
		}
	}

	//lerps glow from start glow to finishing glow
	private void glowTweenForward(){
		glowTimer += Time.deltaTime;
		float currentIntensity = Mathf.Lerp (glowStartingTween, glowEndingTween, glowTimer/glowTweenLength);
		lighting.intensity = currentIntensity;
	}
	//lerps glow from end glow to starting glow
	private void glowTweenBackward(){
		glowTimer += Time.deltaTime;
		float currentIntensity = Mathf.Lerp (glowEndingTween, glowStartingTween, glowTimer/glowTweenLength);
		lighting.intensity = currentIntensity;
	}

	//executes range tween for lights
	private void RangeTweening(){
		if( rangeTween ){
			//checks Timer to see if its time to switch range tween
			if( rangeTimer >= rangeTweenLength ){
				rangeTimer = 0;
				switchRangeTween = !switchRangeTween;
			}

			//lerps range depending on current tween direction and sets range value
			if( switchRangeTween )
				rangeTweenForward();
			if( !switchRangeTween )
				rangeTweenBackward();
			
		}
	}

	//lerps range from start range to end range
	private void rangeTweenForward(){
		rangeTimer += Time.deltaTime;
		float currentRange = Mathf.Lerp ( rangeStartingTween, rangeEndingTween, Time.time);
		lighting.range = currentRange;
	}

	//lerps range from end range to starting range
	private void rangeTweenBackward(){
		rangeTimer += Time.deltaTime;
		float currentRange = Mathf.Lerp ( rangeEndingTween, rangeStartingTween, Time.time);
		lighting.range = currentRange;
	}
}

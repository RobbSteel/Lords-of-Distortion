using UnityEngine;
using System.Collections.Generic;

public class HookHit : MonoBehaviour {
	
	public bool playerhooked = false;
	public GameObject shooter;
	public float timer = 4f;
	public bool returning = false;
    public bool playSound = false;
	public GameObject hookedPlayer;
	public GameObject fireeffect;
	public LineRenderer lr;
	public Material rope;
	public NetworkController networkController;
	public Controller2D affectedPlayerC2D;
	public Vector3 targetPosition;
	public Animator animator;
	public AudioClip hookHitSfx;
	public AudioClip hookWallHitSfx;
	private bool metriconce = false;
	public bool OFFLINE;
	bool poweredUp = false;
	public AudioClip flamehook;
	private bool hitPlatform = false;

	[SerializeField]
	private List<LineRenderer> lines = new List<LineRenderer>();

	void Start()
	{
		returning = false;
		foreach(LineRenderer line in lines)
		{
			line.sortingLayerName = "Player";
			line.sortingOrder =-1;
		}

		animator = GetComponent<Animator>();
	}


	void Update (){
	
		if(playerhooked == true && hookedPlayer != null){
			transform.position = Vector3.MoveTowards(transform.position, hookedPlayer.transform.position, 10);
		}

		if(timer > 0 && !returning)
		{
			timer -= Time.deltaTime;
		} 
		else 
		{
			returning = true;
			if(shooter.GetComponent<Hook>().currentState == Hook.HookState.GoingOut)
			{
				shooter.GetComponent<Hook>().ReturnHook();
			}
		}

        if(shooter.GetComponent<Hook>().currentState == Hook.HookState.GoingOut)
        {
            playSound = false;
        }


	}

	void LateUpdate()
	{
		foreach(LineRenderer line in lines)
		{

			line.SetPosition(0, shooter.GetComponent<Hook>().shootFrom.position);
			line.SetPosition(1, line.transform.position);
		}
	}

	//On collision stops the hook from moving and tells the player to come to the hook
	void OnTriggerEnter2D(Collider2D col){
		if(hitPlatform || playerhooked)
			return;

		if(col.gameObject.tag == "HookGate" & !poweredUp){
			renderer.material.color = Color.red;
			//Added powerhook component and copy info from respective hook gate
			HookGate hookGate = col.gameObject.GetComponent<HookGate>();
			PowerHook pH = gameObject.AddComponent<PowerHook>();
			pH.spawnInfo = new PowerSpawn(hookGate.spawnInfo);
			//just change the type on the fly
			pH.spawnInfo.type = PowerType.POWERHOOK;
			pH.spawnInfo.owner = networkController.theOwner;
			gameObject.tag = "PowerHook";
			poweredUp = true;
			audio.PlayOneShot(flamehook);
			GameObject hookfire = (GameObject)Instantiate(fireeffect, transform.position, transform.rotation);
			hookfire.transform.parent = gameObject.transform;
			return;
		}


		if(col.gameObject.tag != "Player" && col.gameObject.tag != "Power" && !col.isTrigger && !returning){
			if(!OFFLINE){
				if(Analytics.Enabled){
					GA.API.Design.NewEvent("Hooked Environment", col.transform.position);
				}
			}

			rigidbody2D.velocity = Vector2.zero;
            //If hitting a moving platform, the hook moves with the platform
            transform.parent = col.transform;

			animator.SetTrigger("Hooked");
			shooter.GetComponent<Hook>().HitPlatform();
            if (!playSound)
            {
                AudioSource.PlayClipAtPoint(hookWallHitSfx, transform.position);
                playSound = true;
            }
		}

		else if(!poweredUp && col.gameObject.tag == "Player" && col.gameObject != shooter && !returning){

			if(Network.isServer){

				if(Analytics.Enabled && !metriconce){
					GA.API.Design.NewEvent("Hook Hits", col.transform.position);
					metriconce = true;
				}

				hookedPlayer = col.gameObject;
				NetworkController  affectedPlayerNC = hookedPlayer.GetComponent<NetworkController>();
			    
				affectedPlayerC2D = hookedPlayer.GetComponent<Controller2D>();
				affectedPlayerC2D.Hooked(networkController.theOwner);
				rigidbody2D.velocity = Vector2.zero;
				targetPosition = transform.position;
				shooter.GetComponent<Hook>().HitPlayerLocal(affectedPlayerNC.theOwner);
				playerhooked = true; 
				AudioSource.PlayClipAtPoint( hookHitSfx , transform.position );

				animator.SetTrigger("Hooked");

			}
		}
	}
}

// MoveTo.cs
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public enum PlayerSpeedMode {
	isRunning,
	isSneaking,
	isWalking
}

public class MoveTo : MonoBehaviour 
{
	
	public Transform[] points;
	public Transform player;
    public Text gameOver;
	//public bool visualAlertON;
	public float visualAlertRange;
	//public bool acousticAlertON;
	public float run_acousticAlertRange;
	public float walk_acousticAlertRange;
	public float sneak_acousticAlertRange;
	public float stand_acousticAlertRange;
	public float acousticAlertRange;
	public float alertSpeed;
	public float alertCoolDown;
	public Text spotted;
    public Transform spottedAudio;

	private FirstPersonController fpc;
	private int destPoint = 0;
	private NavMeshAgent agent;
	private PlayerSpeedMode playerSpeedMode;
	private float playerSpeed;
	private float playerDistance;
	private float runSpeed;
	private float walkSpeed;
	private bool visualAlert; 
	private float alertStartTime;
    private float haloSize; // zum Färben der Kamera nötig

	//private float playerSpeed;
	//private PlayerSpeedMode playerSpeedMode; 
	
	
	void Start () {

		agent = GetComponent<NavMeshAgent>();

		fpc = GameObject.FindObjectOfType<FirstPersonController>();

        haloSize = 50f;

		//runSpeed = fpc.m_RunSpeed;
		//walkSpeed = fpc.m_WalkSpeed;

		//fpc.m_RunSpeed
		
		// Disabling auto-braking allows for continuous movement
		// between points (ie, the agent doesn't slow down as it
		// approaches a destination point).
		agent.autoBraking = false;
		
		GotoNextPoint();
	}
	
	
	void GotoNextPoint() {
		// Returns if no points have been set up
		if (points.Length == 0)
			return;
		
		// Set the agent to go to the currently selected destination.
		agent.destination = points[destPoint].position;
		
		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % points.Length;
	}


    void Update() {

        playerDistance = Vector3.Distance(agent.transform.position, player.position);

        playerSpeed = fpc.speed;

        SetAcousticAlertRange();

        CheckVisualAlert();

        //Debug.Log (visualAlert);

        if (fpc.isSafe) DeactivateAlertMode();

        if (!fpc.isSafe) { 
            if (playerDistance < acousticAlertRange) {
                ActivateAlertMode();
            } else if (visualAlert) {
                ActivateAlertMode();
            } else if (CheckTimeSinceAlert()) {
                agent.speed = 8;
                agent.destination = points[destPoint].position;
            }
        }

        if(playerDistance < 50 )
        {
            fpc.GetComponent<CameraGUI>().setNPCDistance(playerDistance);
        }
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (agent.remainingDistance < 0.5f)
			GotoNextPoint();

	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "FPSController")
        {
            gameOver.enabled = true;
        }
    }

    void CheckVisualAlert() {
		RaycastHit hit;

		Vector3 fwd = agent.transform.TransformDirection (Vector3.forward);
		Vector3 targetDir = player.transform.position - agent.transform.position;
	
		Debug.DrawRay (agent.transform.position, targetDir, Color.cyan);
		Debug.DrawRay (agent.transform.position, fwd * visualAlertRange, Color.yellow);
	

		/*if (Physics.Raycast(agent.transform.position, fwd, out hit, 50.0F)) {
			Debug.Log (hit.collider.gameObject.name);

		}*/

		if (Vector3.Angle (targetDir, agent.transform.forward) < 80) {
			if (Physics.Raycast(agent.transform.position, targetDir, out hit, visualAlertRange)) {
				if (hit.collider.gameObject.name.Equals("Player")) {
					Debug.Log("Player has been seen");
					visualAlert = true;
				} else {
					visualAlert = false;
				}
			}
		}
	}

    void SetAcousticAlertRange () {
		if (fpc.m_IsRunning) {
			acousticAlertRange = run_acousticAlertRange;
		} else if (fpc.m_IsSneaking) {
			acousticAlertRange = sneak_acousticAlertRange;
		} else if (fpc.m_IsStandingStill) {
			acousticAlertRange = stand_acousticAlertRange;
		} else {
			acousticAlertRange = walk_acousticAlertRange;
		}
	}

	void ActivateAlertMode() {
		    agent.destination = player.position;
		    agent.speed = alertSpeed;

		    alertStartTime = Time.time;

            AudioSource audio = spottedAudio.GetComponent<AudioSource>();
            audio.Play();
            spotted.enabled = true;
    }

	public void DeactivateAlertMode() {
		Debug.Log ("----------------------");
		Debug.Log ("deactivate");
		agent.destination = points [destPoint].position;
        spotted.enabled = false;
    }

	bool CheckTimeSinceAlert(){
		if (Time.time - alertStartTime > alertCoolDown) {
			return true;
		} else {
			spotted.enabled = false;
			return false;
		}
	}
}
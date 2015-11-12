// MoveTo.cs
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerSpeedMode {
	isRunning,
	isSneaking,
	isWalking
}

public class MoveTo : MonoBehaviour 
{
	
	public Transform[] points;
	public Transform player;
	public float visualAlertRange;
	public float acousticAlertRange;



	private FirstPersonController fpc;
	private int destPoint = 0;
	private NavMeshAgent agent;
	private bool alertMode;
	private PlayerSpeedMode playerSpeedMode;
	private float playerSpeed;
	private float playerDistance;
	private float runSpeed;
	private float walkSpeed;

	//private float playerSpeed;
	//private PlayerSpeedMode playerSpeedMode; 
	
	
	void Start () {

		agent = GetComponent<NavMeshAgent>();

		fpc = GameObject.FindObjectOfType<FirstPersonController>();

		runSpeed = fpc.m_RunSpeed;
		walkSpeed = fpc.m_WalkSpeed;

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
	
	
	void Update () {

		playerDistance = Vector3.Distance (agent.transform.position, player.position);

		playerSpeed = fpc.speed;

		SetAcousticAlertRange();
	
		if (playerDistance < acousticAlertRange ) {
			agent.destination = player.position;
		} else {
			agent.destination = points [destPoint].position;
		}
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (agent.remainingDistance < 0.5f)
			GotoNextPoint();

	}

	bool CheckVisualAlert () {
		return (playerDistance < visualAlertRange);		
	}

	/*bool CheckAcousticAlert () {

		if () {
			return true;
		}

		return false;
		
	}
*/
	void SetAcousticAlertRange () {
		if (fpc.m_IsRunning) {
			acousticAlertRange = 20;
		} else if (fpc.m_IsSneaking) {
			acousticAlertRange = 7;
		} else {
			acousticAlertRange = 13;
		}
	}

}
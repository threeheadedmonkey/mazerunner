using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class SafePlace : MonoBehaviour {

	public Transform player;
	public FirstPersonController fps;
	public MoveTo agent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "FPSController")
		{	
			Debug.Log("----------------------");
			Debug.Log("-----  SAVEPLACE");
			Debug.Log("----------------------");

			//agent.DeactivateAlertMode();
		}
		
		//gameOver.enabled = true;
		
		/*   if (col.gameObject.name == "FPSController")
           {
               screenColor = blueScreen.GetComponent<Renderer>().material.color;
               screenColor.a = 1.0f;
               blueScreen.GetComponent<Renderer>().material.color = screenColor;
           }*/
	}
}

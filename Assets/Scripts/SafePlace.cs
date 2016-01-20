using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class SafePlace : MonoBehaviour {

	public Transform player;
	public FirstPersonController fps;
	public MoveTo agent;
    public Text safePlaceText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("test");
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "FPSController")
		{	
			Debug.Log("----------------------");
			Debug.Log("-----  SAVEPLACE");
			Debug.Log("----------------------");
            safePlaceText.enabled = true;
            fps.isSafe = true;

			//agent.DeactivateAlertMode();
		}
	}

    void OnCollisionExit(Collision col)
    {
        Debug.Log("no more safe place");
        fps.isSafe = false;
        safePlaceText.enabled = false;
    }
}

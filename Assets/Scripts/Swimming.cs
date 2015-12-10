using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class Swimming : MonoBehaviour {

    public Transform player;
    public FirstPersonController fps;
    public Transform blueScreen;
    public Text gameOver;
    
    private Color screenColor;

	// Use this for initialization
	void Start () {
        screenColor = blueScreen.GetComponent<Renderer>().material.color;
        screenColor.a = 0.0f;
        blueScreen.GetComponent<Renderer>().material.color = screenColor;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "FPSController")
        {
            if( !fps.m_IsSwimmer ) gameOver.enabled = true;
        }

        //gameOver.enabled = true;

        /*   if (col.gameObject.name == "FPSController")
           {
               screenColor = blueScreen.GetComponent<Renderer>().material.color;
               screenColor.a = 1.0f;
               blueScreen.GetComponent<Renderer>().material.color = screenColor;
           }*/
    }

    void OnCollisionExit(Collision col)
    {
      /*  if (col.gameObject.name == "FPSController")
        {
            screenColor = blueScreen.GetComponent<Renderer>().material.color;
            screenColor.a = 0.0f;
            blueScreen.GetComponent<Renderer>().material.color = screenColor;
        }*/
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class CameraGUI : MonoBehaviour {

    //public float distanceToFinish;
    public FirstPersonController controller;
    public Transform player;
    public Transform finish;
    public Text distanceDisplay;
    public Text gameOver;
    public Text cooldownDisplay;
	public Text spotted;
    public Text safePlaceText;
    public Transform redScreen;


    private float distanceLeft;
    private float coolDown;
    private string currentIdentity;
    private float distanceToNPC;
    private Color screenColor;
    private bool isSwimming;
    // Vector3 lastPosition;

    // Use this for initialization
    void Start()
    {
        gameOver.enabled = false;
		spotted.enabled = false;
        safePlaceText.enabled = false;
        distanceLeft = Vector3.Distance(player.position, finish.position);

        distanceToNPC = 100;

        screenColor = redScreen.GetComponent<SpriteRenderer>().material.color;
        screenColor.a = 0.0f;
        redScreen.GetComponent<SpriteRenderer>().material.color = screenColor;

        isSwimming = false;

    }

    // Update is called once per frame
    void Update()
    {
        distanceLeft = Vector3.Distance(player.position, finish.position);
        distanceDisplay.text = distanceLeft.ToString("F0");
        
        if( controller.m_IsClimber) { 
            currentIdentity = "Jumper";
        } else if (controller.m_IsRunner) {
            currentIdentity = "Runner";
        } else if( controller.m_IsSwimmer) {
            currentIdentity = "Swimmer";
        }
        // coolDown = controller.GetRemainingCoolDownTime();
        cooldownDisplay.text = currentIdentity + ": " +
                Mathf.Round(controller.GetRemainingCoolDownTime() * 100f) / 100f;

        // alpha channel des roten Screens wird durch Gegner-Distanz bestimmt
        if( distanceToNPC < 50) {
               screenColor = redScreen.GetComponent<Renderer>().material.color;
               screenColor.a = 0.5f - distanceToNPC/100f;
               redScreen.GetComponent<Renderer>().material.color = screenColor;
        } else {
               screenColor = redScreen.GetComponent<Renderer>().material.color;
               screenColor.a = 0.0f;
               redScreen.GetComponent<Renderer>().material.color = screenColor;
        }
    }

    // Distanz wird in NPC Klasse berechnet und hier gesetzt
    public void setNPCDistance(float npcDistance) {
        distanceToNPC = npcDistance;
    }
}

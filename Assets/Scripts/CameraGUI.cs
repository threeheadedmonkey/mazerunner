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
    private float distanceLeft = 0;
    private float coolDown;
    private string currentIdentity;
    // Vector3 lastPosition;

    // Use this for initialization
    void Start()
    {
        gameOver.enabled = false;
        distanceLeft = Vector3.Distance(player.position, finish.position);
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
                Mathf.Round(controller.GetRemainingCoolDownTime() * 100f) / 100f; ;
    }

    public void colorByNPCDistance()
    {

    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraGUI : MonoBehaviour {

    //public float distanceToFinish;
    public Transform player;
    public Transform finish;
    public Text distanceDisplay;
    public Text gameOver;
    float distanceLeft = 0;
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
    }
}

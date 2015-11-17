using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayDistanceToFinish : MonoBehaviour {
    //public float distanceToFinish;
    public Transform player;
    public Transform finish;
    public Text label;
    float distanceLeft = 0;
   // Vector3 lastPosition;

	// Use this for initialization
	void Start () {
        distanceLeft = Vector3.Distance(player.position, finish.position);
	}
	
	// Update is called once per frame
	void Update () {
        distanceLeft = Vector3.Distance(player.position, finish.position);
        label.text = distanceLeft.ToString("F0");
	}
}

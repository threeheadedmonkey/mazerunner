using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayDistanceToFinish : MonoBehaviour {
    //public float distanceToFinish;
    public Transform player;
    public Transform finish;
	public Slider distanceSlider;
    public Text label;
    private float distanceLeft = 0;
	private float startDistance;


   // Vector3 lastPosition;

	// Use this for initialization
	void Start () {
		startDistance = Vector3.Distance(player.position, finish.position);
        distanceLeft = Vector3.Distance(player.position, finish.position);
	}
	
	// Update is called once per frame
	void Update () {
        distanceLeft = Vector3.Distance(player.position, finish.position);
		distanceSlider.value = distanceLeft / startDistance;
        label.text = distanceLeft.ToString("F0");
	}
}

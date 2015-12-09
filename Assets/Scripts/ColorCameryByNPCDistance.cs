using UnityEngine;
using System.Collections;

public class ColorCameryByNPCDistance : MonoBehaviour {
    public Transform colorPlane;

    private Material planeMaterial;
    private Color color;

	// Use this for initialization
	void Start () {

        //colorPlane.transform.LookAt(GetComponent<Camera>().transform.position);
        color = colorPlane.GetComponent<Renderer>().material.color;
        color.a = 0.5f;
        colorPlane.GetComponent<Renderer>().material.color = color;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

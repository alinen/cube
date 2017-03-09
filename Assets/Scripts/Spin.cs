using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

    public Vector3 axis;
    public float rate;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.localRotation = Quaternion.AngleAxis(rate * Time.deltaTime, axis) * transform.localRotation;
	}
}

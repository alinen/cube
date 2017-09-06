using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {

    public float radius = 3.0f;
    public Vector3 target = new Vector3(0, 0, 0);
    public float rate = 10.0f;
    private float _angle = 0;
    private float _inc = 0;

	// Use this for initialization
	void Start () {
        _inc = rate * Mathf.Deg2Rad;	
	}
	
	// Update is called once per frame
	void Update () {
        float x = Mathf.Cos(_angle);
        float z = Mathf.Sin(_angle);
        float y = Mathf.Cos(_angle);
        _angle += _inc * Time.deltaTime;
        Vector3 p = new Vector3(x, y, z).normalized * radius;
        transform.position = p;
        transform.LookAt(target);
	}
}

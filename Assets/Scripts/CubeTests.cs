using UnityEngine;
using System.Collections;

public class CubeTests : MonoBehaviour {

    CubePlanner _solver;

	// Use this for initialization
	void Start ()
    {
        _solver = gameObject.GetComponent<CubePlanner>();
	}

    void Update()
    {
        // Set cube to home state
        // 

    } 
}

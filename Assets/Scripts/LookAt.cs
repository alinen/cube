using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    public float radius = 4.0f;
    public float rate = 0.001f;

	void Start ()
    {
        SetTarget(target);
	}
	
    public void SetTarget(Transform t)
    {
        target = t;
    }

	void Update ()
    {
        Vector3 targetPos = target.position.normalized * radius;
        transform.position = Vector3.Lerp(transform.position, targetPos, Mathf.Pow(rate, Time.deltaTime));
        transform.rotation = Quaternion.LookRotation(-targetPos);
	}
}

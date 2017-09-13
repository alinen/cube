using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    public float radius = 4.0f;
    public float rate = 5.0f;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _time;

	void Start ()
    {
        transform.position = transform.position.normalized * radius;
        SetTarget(target);
	}
	
    public void SetTarget(Vector3 t)
    {
        target = null;
        _startPos = transform.position;
        _targetPos = t.normalized * radius;
        _time = 0;
    }

    public void SetTarget(Transform t)
    {
        target = t;
        _startPos = transform.position;
        _time = 0;
    }

	void Update ()
    {
        Vector3 targetPos = _targetPos;
        if (target)
        {
            targetPos = target.position.normalized * radius;
        }

        _time += Time.deltaTime;
        if (_time < rate)
        {
            float frac = _time / rate;
            transform.position = Vector3.Slerp(_startPos, targetPos, frac);
        }
        else
        {
            transform.position = targetPos;
        }
        transform.rotation = Quaternion.LookRotation(-transform.position, Camera.main.transform.up);
	}
}

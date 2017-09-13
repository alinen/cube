using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public Transform cube;
    private CubePlanner _planner;
    private CubeController _controller;
    private Text _taskText;
    private Text _stepText;
    private Text _currentText;

	void Start ()
    {
        _planner = cube.GetComponent<CubePlanner>();
        _controller = cube.GetComponent<CubeController>();
        _currentText = transform.Find("Current").GetComponent<Text>();
        _stepText = transform.Find("Step").GetComponent<Text>();
        _taskText = transform.Find("Task").GetComponent<Text>();
	}
	
	void Update ()
    {
        _taskText.text = _planner.GetCurrentTaskRow();
        _stepText.text = _planner.GetCurrentTaskName();
        _currentText.text = _controller.GetCommandString();
        _currentText.supportRichText = true;

    }
}

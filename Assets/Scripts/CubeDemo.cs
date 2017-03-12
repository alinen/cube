using UnityEngine;
using System.Collections;

public class CubeDemo : MonoBehaviour {

    // F L F U' R U F2 L2 U' L' B D' B' L2 U Mini Cube
    // F U F R L2 B D' R D2 L D' B R2 L F U F VStripes
    // R2 L' D F2 R' D' R' L U' D R D B2 R' U D2 Cross 
    // L U B' U' R L' B R' F B' D R D' F' Anaconda
    // F2 R' B' U R' L F' L F' B D' R B L2 Python
    // R2 L F' R L' D R' U D' B U' R' D' Mamba

    public float turnRate = 20.0f;
    public string sequence = "R L F B R L F B R L F B"; // zigzag

    private CubeController _controller;
    private bool _initialized = false;


    // Use this for initialization
    void Start ()
    {
	}

    public void ParseRecipe(string s)
    {
        _controller.ClearQueue();

        char[] delimiterChars = { ' ' };
        string[] words = s.Split(delimiterChars);
        for (int w = 0; w < words.Length; w++)
        {
            string word = words[w];
            _controller.QueueCommand(word, turnRate);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (!_initialized)
        {
            _controller = gameObject.GetComponent<CubeController>();
            ParseRecipe(sequence);
            _initialized = true;
        }
	}
}

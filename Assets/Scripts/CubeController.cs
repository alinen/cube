﻿using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour {

    // F L F U' R U F2 L2 U' L' B D' B' L2 U Mini Cube
    // F U F R L2 B D' R D2 L D' B R2 L F U F VStripes
    // R2 L' D F2 R' D' R' L U' D R D B2 R' U D2 Cross 
    // L U B' U' R L' B R' F B' D R D' F' Anaconda
    // F2 R' B' U R' L F' L F' B D' R B L2 Python
    // R2 L F' R L' D R' U D' B U' R' D' Mamba

    public float turnRate = 20.0f;
    public string sequence = "R L F B R L F B R L F B"; // zigzag
    public bool loop = false;

    public abstract class Animator
    {
        public Animator() { }
        public abstract void Update();
        public abstract void Start();
        public abstract bool IsDone();
    };

    public class Spiner : Animator
    {
        private Transform _t;
        private Vector3 _axis;
        private float _rate;

        public Spiner(Transform root, Vector3 axis, float rate)
        {
            _t = root;
            _axis = axis;
            _rate = rate;
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            _t.localRotation = Quaternion.AngleAxis(_rate * Time.deltaTime, _axis) * _t.localRotation;
        }

        public override bool IsDone()
        {
            return false;
        }
    }

    public class Pauser : Animator
    {
        private float _delay;
        private float _amount;

        public override void Start()
        {
            _amount = 0.0f;
        }

        public Pauser(float delay)
        {
            _delay = delay;
        }

        public override void Update()
        {
            _amount += Time.deltaTime;
        }

        public override bool IsDone()
        {
            if (_amount > _delay) return true;
            return false;
        }
    }

    public class Rotator : Animator
    {
        private ArrayList _list;
        private Vector3 _center;
        private Vector3 _axis;
        private float _rate;
        private float _amount;
        private float _delta;
        private ArrayList _targets = new ArrayList();

        public Rotator(ArrayList list, Vector3 center, Vector3 axis, float amount, float rate = 40)
        {
            _list = list;
            _center = center;
            _axis = axis;
            _amount = amount;
            _rate = rate;
            Debug.Log("Create " + axis + " " + amount);
        }

        public override void Start()
        {
            _targets.Clear();
            foreach (Transform t in _list)
            {
                Quaternion q = Quaternion.AngleAxis(_amount, _axis) * t.localRotation;
                _targets.Add(q);
            } 
            _delta = 0.0f;
            //Debug.Log(_axis + " " + _amount);
        }

        public override void Update()
        {
            CubeController.turn(_list, _center, _axis, _rate);
            _delta += _rate * Time.deltaTime;
        }

        public override bool IsDone()
        {
            if (_delta > _amount)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    Transform t = _list[i] as Transform;
                    t.localRotation = (Quaternion) _targets[i];
                }
                return true;
            }
            return false;
        }
    }
    private ArrayList rotations = new ArrayList();
    private int current;
    
    private ArrayList posX = new ArrayList();
    private ArrayList negX = new ArrayList();
    private ArrayList zeroX = new ArrayList();

    private Vector3 cPosX = new Vector3();
    private Vector3 cNegX = new Vector3();
    private Vector3 cZeroX = new Vector3();

    private ArrayList posY = new ArrayList();
    private ArrayList negY = new ArrayList();
    private ArrayList zeroY = new ArrayList();

    private Vector3 cPosY = new Vector3();
    private Vector3 cNegY = new Vector3();
    private Vector3 cZeroY = new Vector3();

    private ArrayList posZ = new ArrayList();
    private ArrayList negZ = new ArrayList();
    private ArrayList zeroZ = new ArrayList();

    private Vector3 cPosZ = new Vector3();
    private Vector3 cNegZ = new Vector3();
    private Vector3 cZeroZ = new Vector3();

	void Start ()
    {
        SortCubeGroups();

        cZeroX = ComputeCenter(zeroX);
        cPosX = ComputeCenter(posX);
        cNegX = ComputeCenter(negX);

        cZeroY = ComputeCenter(zeroY);
        cPosY = ComputeCenter(posY);
        cNegY = ComputeCenter(negY);

        cZeroZ = ComputeCenter(zeroZ);
        cPosZ = ComputeCenter(posZ);
        cNegZ = ComputeCenter(negZ);

        //string zigzag = "R L F B R L F B R L F B";
        parseRecipe(sequence);

        current = 0;
        Animator rotator = rotations[0] as Animator;
        rotator.Start();
    }

    void parseRecipe(string s)
    {
        rotations.Clear();

        char[] delimiterChars = { ' ' };
        string[] words = s.Split(delimiterChars);
        for (int w = 0; w < words.Length; w++)
        {
            string word = words[w];
            Debug.Log(word);
            // NOTE: May seem unintuitive but remember Unity's LHS
            if (word == "F")  rotations.Add(new Rotator(posX, cPosX, new Vector3( 1, 0, 0),  90, turnRate));
            if (word == "F'") rotations.Add(new Rotator(posX, cPosX, new Vector3(-1, 0, 0),  90, turnRate));
            if (word == "F2") rotations.Add(new Rotator(posX, cPosX, new Vector3( 1, 0, 0), 180, turnRate));
            if (word == "B")  rotations.Add(new Rotator(negX, cNegX, new Vector3(-1, 0, 0),  90, turnRate));
            if (word == "B'") rotations.Add(new Rotator(negX, cNegX, new Vector3( 1, 0, 0),  90, turnRate));
            if (word == "B2") rotations.Add(new Rotator(negX, cNegX, new Vector3(-1, 0, 0), 180, turnRate));
            if (word == "U")  rotations.Add(new Rotator(posY, cPosY, new Vector3( 0, 1, 0),  90, turnRate));
            if (word == "U'") rotations.Add(new Rotator(posY, cPosY, new Vector3( 0,-1, 0),  90, turnRate));
            if (word == "U2") rotations.Add(new Rotator(posY, cPosY, new Vector3( 0, 1, 0), 180, turnRate));
            if (word == "D")  rotations.Add(new Rotator(negY, cNegY, new Vector3( 0,-1, 0),  90, turnRate));
            if (word == "D'") rotations.Add(new Rotator(negY, cNegY, new Vector3( 0, 1, 0),  90, turnRate));
            if (word == "D2") rotations.Add(new Rotator(negY, cNegY, new Vector3( 0,-1, 0), 180, turnRate));
            if (word == "R")  rotations.Add(new Rotator(posZ, cPosZ, new Vector3( 0, 0, 1),  90, turnRate));
            if (word == "R'") rotations.Add(new Rotator(posZ, cPosZ, new Vector3( 0, 0,-1),  90, turnRate));
            if (word == "R2") rotations.Add(new Rotator(posZ, cPosZ, new Vector3( 0, 0, 1), 180, turnRate));
            if (word == "L")  rotations.Add(new Rotator(negZ, cNegZ, new Vector3( 0, 0,-1),  90, turnRate));
            if (word == "L'") rotations.Add(new Rotator(negZ, cNegZ, new Vector3( 0, 0, 1),  90, turnRate));
            if (word == "L2") rotations.Add(new Rotator(negZ, cNegZ, new Vector3( 0, 0,-1), 180, turnRate));
        }

        rotations.Add(new Spiner(transform, Vector3.up, 100.0f));
    }

    void SortCubeGroups()
    {
        zeroX.Clear();
        zeroY.Clear();
        zeroZ.Clear();

        posX.Clear();
        posY.Clear();
        posZ.Clear();

        negX.Clear();
        negY.Clear();
        negZ.Clear();

        foreach (Transform child in transform)
        {
            Check(child, 0,  0.0f, zeroX);
            Check(child, 0,  1.25f, posX);
            Check(child, 0, -1.25f, negX);

            Check(child, 1,  0.0f, zeroY);
            Check(child, 1,  1.25f, posY);
            Check(child, 1, -1.25f, negY);

            Check(child, 2,  0.0f, zeroZ);
            Check(child, 2,  1.25f, posZ);
            Check(child, 2, -1.25f, negZ);
        }

    }

    void Check(Transform t, int idx, float desired, ArrayList list)
    {
        Vector3 pos = t.localPosition;
        if (Mathf.Abs(pos[idx] - desired) < 0.25f)
        {
            // Adjust position so it's "perfect"
            pos[idx] = desired;
            t.localPosition = pos;
            list.Add(t);
        }
    }

    Vector3 ComputeCenter(ArrayList transforms)
    {
        Vector3 ave = new Vector3(0, 0, 0);
        foreach (Transform t in transforms)
        {
            ave += t.localPosition;
        }
        ave /= transforms.Count;
        Debug.Log(ave);
        return ave;
    }

    public static void turn(ArrayList list, Vector3 center, Vector3 axis, float rate)
    {
        foreach (Transform t in list)
        {
            t.RotateAround(center, axis, rate * Time.deltaTime);
        }
    }

    void Update ()
    {
        Animator rotator = rotations[current] as Animator;
        rotator.Update();
        if (rotator.IsDone())
        {
            SortCubeGroups();

            current = (current + 1) % rotations.Count;
            rotator = rotations[current] as Animator;
            rotator.Start();
        }
	}
}

﻿using UnityEngine;
using System.Collections;

public class CubeStateManager 
{
    private Transform _root = null;

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

    public CubeStateManager() { }

    public void init(Transform root)
    {
        _root = root;

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
    }

    public void CmdToTurn(string word, out ArrayList list, out Vector3 center, out Vector3 axis, out float amount)
    {
        list = new ArrayList();
        center = new Vector3();
        axis = new Vector3();
        amount = 0.0f;

        if (word == "F") { list = posX; center = cPosX; axis = new Vector3(1, 0, 0); amount = 90; }
        else if (word == "F'") { list = posX; center = cPosX; axis = new Vector3(-1, 0, 0); amount = 90; }
        else if (word == "F2") { list = posX; center = cPosX; axis = new Vector3(1, 0, 0); amount = 180; }
        else if (word == "B") { list = negX; center = cNegX; axis = new Vector3(-1, 0, 0); amount = 90; }
        else if (word == "B'") { list = negX; center = cNegX; axis = new Vector3(1, 0, 0); amount = 90; }
        else if (word == "B2") { list = negX; center = cNegX; axis = new Vector3(-1, 0, 0); amount = 180; }
        else if (word == "U") { list = posY; center = cPosY; axis = new Vector3(0, 1, 0); amount = 90; }
        else if (word == "U'") { list = posY; center = cPosY; axis = new Vector3(0, -1, 0); amount = 90; }
        else if (word == "U2") { list = posY; center = cPosY; axis = new Vector3(0, 1, 0); amount = 180; }
        else if (word == "D") { list = negY; center = cNegY; axis = new Vector3(0, -1, 0); amount = 90; }
        else if (word == "D'") { list = negY; center = cNegY; axis = new Vector3(0, 1, 0); amount = 90; }
        else if (word == "D2") { list = negY; center = cNegY; axis = new Vector3(0, -1, 0); amount = 180; }
        else if (word == "R") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, 1); amount = 90; }
        else if (word == "R'") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, -1); amount = 90; }
        else if (word == "R2") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, 1); amount = 180; }
        else if (word == "L") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, -1); amount = 90; }
        else if (word == "L'") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, 1); amount = 90; }
        else if (word == "L2") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, -1); amount = 180; }
        else Debug.Log("Unknown command passed to CmdToTurn!");
    }

    public void SortCubeGroups()
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

        foreach (Transform child in _root)
        {
            Check(child, 0, 0.0f, zeroX);
            Check(child, 0, 1.25f, posX);
            Check(child, 0, -1.25f, negX);

            Check(child, 1, 0.0f, zeroY);
            Check(child, 1, 1.25f, posY);
            Check(child, 1, -1.25f, negY);

            Check(child, 2, 0.0f, zeroZ);
            Check(child, 2, 1.25f, posZ);
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
        //Debug.Log(ave);
        return ave;
    }

    // Perform an instant turn
    public void turn(ArrayList list, Vector3 center, Vector3 axis, float amount)
    {
        foreach (Transform t in list)
        {
            Quaternion q = Quaternion.AngleAxis(amount, axis) * t.localRotation;
            t.RotateAround(center, axis, amount);
            t.localRotation = q;
        }
        SortCubeGroups();
    }
}

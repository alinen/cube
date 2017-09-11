using UnityEngine;
using System.Collections.Generic;

public class CubeStateManager 
{
    private Transform _root = null;

    // left to right vertical stacks
    private List<Transform> posX = new List<Transform>();
    private List<Transform> negX = new List<Transform>();
    private List<Transform> zeroX = new List<Transform>();

    // pivots for rotating X stacks 
    private Vector3 cPosX = new Vector3();
    private Vector3 cNegX = new Vector3();
    private Vector3 cZeroX = new Vector3();

    // top to bottom horizontal stacks
    private List<Transform> posY = new List<Transform>();
    private List<Transform> negY = new List<Transform>();
    private List<Transform> zeroY = new List<Transform>();

    // pivots for rotating Y stacks 
    private Vector3 cPosY = new Vector3();
    private Vector3 cNegY = new Vector3();
    private Vector3 cZeroY = new Vector3();

    // back to front vertical stacks
    private List<Transform> posZ = new List<Transform>();
    private List<Transform> negZ = new List<Transform>();
    private List<Transform> zeroZ = new List<Transform>();

    // pivots for rotating Z stacks 
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

    public void turn(string word)
    {
        List<Transform> list;
        Vector3 center;
        Vector3 axis;
        float amount;

        CmdToTurn(word, out list, out center, out axis, out amount);
        if (list.Count > 0) turn(list, center, axis, amount);
    }

    public string Reverse(string word)
    {
        string turn = "";
        if (word == "F") turn = "F'";
        else if (word == "F'") turn = "F";
        else if (word == "F2") turn = "F2";
        else if (word == "B") turn = "B'";
        else if (word == "B'") turn = "B";
        else if (word == "B2") turn = "B2";
        else if (word == "U") turn = "U'";
        else if (word == "U'") turn = "U";
        else if (word == "U2") turn = "U2";
        else if (word == "D") turn = "D'";
        else if (word == "D'") turn = "D";
        else if (word == "D2") turn = "D2";
        else if (word == "R") turn = "R'";
        else if (word == "R'") turn = "R";
        else if (word == "R2") turn = "R2";
        else if (word == "L") turn = "L'";
        else if (word == "L'") turn = "L";
        else if (word == "L2") turn = "L2";
        else Debug.Log("Unknown command passed to Reverse!");
        return turn;
    }

    public void CmdToTurn(string word, out List<Transform> list, out Vector3 center, out Vector3 axis, out float amount)
    {
        list = new List<Transform>();
        center = new Vector3();
        axis = new Vector3();
        amount = 0.0f;

        if (word == "F") { list = posX; center = cPosX; axis = new Vector3(1, 0, 0); amount = 90; Test(posX, 0, 1.25f); }
        else if (word == "F'") { list = posX; center = cPosX; axis = new Vector3(-1, 0, 0); amount = 90; Test(posX, 0, 1.25f); }
        else if (word == "F2") { list = posX; center = cPosX; axis = new Vector3(1, 0, 0); amount = 180; Test(posX, 0, 1.25f); }
        else if (word == "B") { list = negX; center = cNegX; axis = new Vector3(-1, 0, 0); amount = 90; Test(negX, 0, -1.25f); }
        else if (word == "B'") { list = negX; center = cNegX; axis = new Vector3(1, 0, 0); amount = 90; Test(negX, 0, -1.25f); }
        else if (word == "B2") { list = negX; center = cNegX; axis = new Vector3(-1, 0, 0); amount = 180; Test(negX, 0, -1.25f); }
        else if (word == "U") { list = posY; center = cPosY; axis = new Vector3(0, 1, 0); amount = 90; Test(posY, 1, 1.25f); }
        else if (word == "U'") { list = posY; center = cPosY; axis = new Vector3(0, -1, 0); amount = 90; Test(posY, 1, 1.25f); }
        else if (word == "U2") { list = posY; center = cPosY; axis = new Vector3(0, 1, 0); amount = 180; Test(posY, 1, 1.25f); }
        else if (word == "D") { list = negY; center = cNegY; axis = new Vector3(0, -1, 0); amount = 90; Test(negY, 1, -1.25f); }
        else if (word == "D'") { list = negY; center = cNegY; axis = new Vector3(0, 1, 0); amount = 90; Test(negY, 1, -1.25f); }
        else if (word == "D2") { list = negY; center = cNegY; axis = new Vector3(0, -1, 0); amount = 180; Test(negY, 1, -1.25f); }
        else if (word == "R") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, 1); amount = 90; Test(posZ, 2, 1.25f); }
        else if (word == "R'") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, -1); amount = 90; Test(posZ, 2, 1.25f); }
        else if (word == "R2") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, 1); amount = 180; Test(posZ, 2, 1.25f); }
        else if (word == "L") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, -1); amount = 90; Test(negZ, 2, -1.25f); }
        else if (word == "L'") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, 1); amount = 90; Test(negZ, 2, -1.25f); }
        else if (word == "L2") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, -1); amount = 180; Test(negZ, 2, -1.25f); }
        else Debug.Log("Unknown command passed to CmdToTurn! "+word);
    }

    void Test(List<Transform> list, int idx, float dpos)
    { 
        // Move to test class at some point
        // https://blogs.unity3d.com/2014/05/21/unit-testing-part-1-unit-tests-by-the-book/
        // https://blogs.unity3d.com/2014/06/03/unit-testing-part-2-unit-testing-monobehaviours/

        foreach (Transform t in list)
        {
            Debug.Assert(Mathf.Abs(t.position[idx] - dpos) < 0.001f);
        }
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

            CheckAngle(child, 0);
            CheckAngle(child, 1);
            CheckAngle(child, 2);
        }

        Debug.Assert(zeroX.Count == 8);
        Debug.Assert(zeroY.Count == 8);
        Debug.Assert(zeroZ.Count == 8);
        Debug.Assert(posX.Count == 9);
        Debug.Assert(posY.Count == 9);
        Debug.Assert(posZ.Count == 9);
        Debug.Assert(negX.Count == 9);
        Debug.Assert(negY.Count == 9);
        Debug.Assert(negZ.Count == 9);
    }

    void CheckAngle(Transform t, int idx)
    {
        Vector3 euler = t.localEulerAngles;
        float sign = Mathf.Sign(euler[idx]);
        if (Mathf.Abs(euler[idx] - 90) < 10) euler[idx] = sign * 90;
        if (Mathf.Abs(euler[idx] - 270) < 10) euler[idx] = sign * 270;
        if (Mathf.Abs(euler[idx] - 0) < 10) euler[idx] = 0;
        if (Mathf.Abs(euler[idx] - 180) < 10) euler[idx] = sign * 180;
        t.localEulerAngles = euler;
    }

    void Check(Transform t, int idx, float desired, List<Transform> list)
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

    Vector3 ComputeCenter(List<Transform> transforms)
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
    public void turn(List<Transform> list, Vector3 center, Vector3 axis, float amount)
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

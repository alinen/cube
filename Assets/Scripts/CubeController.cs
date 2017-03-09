using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour {

    public Transform root = null;
    class Rotator
    {
        void Update()
        {
        }

        bool IsDone()
        {
            return false;
        }
    }
    private ArrayList rotations = new ArrayList();
    
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

	void Start () {
        
        foreach (Transform child in root)
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
            ave += t.position;
        }
        ave /= transforms.Count;
        Debug.Log(ave);
        return ave;
    }

    void turn(ArrayList list, Vector3 center, Vector3 axis, float rate)
    {
        foreach (Transform t in list)
        {
            t.RotateAround(center, axis, rate * Time.deltaTime);
        }
    }

    void Update ()
    {
        //turn(negX, cNegX, new Vector3(-1, 0, 0), 15);
        //turn(posX, cPosX, new Vector3( 1, 0, 0), 20);
        //turn(zeroX, cZeroX, new Vector3( 1, 0, 0), 25);

        turn(negY, cNegY, new Vector3(0, -1, 0), 30);
        turn(posY, cPosY, new Vector3( 0, 1, 0), 20);
        //turn(zeroY, cZeroY, new Vector3( 0, 1, 0), 25);

        //turn(negZ, cNegZ, new Vector3( 0, 0, -1), 15);
        //turn(posZ, cPosZ, new Vector3( 0, 0, 1), 20);
        //turn(zeroZ, cZeroZ, new Vector3( 0, 0, 1), 25);
	}
}

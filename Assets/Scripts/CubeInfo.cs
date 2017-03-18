using UnityEngine;
using System.Collections;

public class CubeInfo 
{
    public class Cubie
    {
        public int id;
        public Transform transform;
        public Vector3 homePos;
        public Vector3 homeRot;
    }
    private ArrayList _cubeInfos = new ArrayList();

    public CubeInfo() { }

    public void init (Transform homeStateCube)
    {
        Debug.Log("Initialze cube info");
        int index = 0;
        foreach (Transform t in homeStateCube)
        {
            Cubie info = new Cubie();
            info.id = index++;
            info.transform = t;
            info.homePos = t.localPosition;
            info.homeRot = t.localRotation.eulerAngles;
            Debug.Log(t.transform.parent.gameObject.name+" "+t.gameObject.name+" "+info.id + " " + info.homePos + " " + info.homeRot);
            _cubeInfos.Add(info);
        }
	}

    public int GetNumCubes()
    {
        return _cubeInfos.Count;
    }

    public Cubie GetCubeInfo(int index)
    {
        return _cubeInfos[index] as Cubie;
    }

    public Cubie FindCube(Vector3 homePos)
    {
        foreach (Cubie info in _cubeInfos)
        {
            // ok, equality should be perfect
            if (info.homePos == homePos) return info;
        }
        return null;
    }

    public bool IsMiddleCube(int id)
    {
        Cubie info = _cubeInfos[id] as Cubie;

        int zeroCount = 0;
        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(info.homePos[i]) < 0.001f) zeroCount++;
        }

        return zeroCount == 2;
    }

    public ArrayList FindTopMiddle()
    {
        // top is Y = 1.25
        // middle is X or Z == 0
        ArrayList result = new ArrayList();
        foreach (Cubie info in _cubeInfos)
        {
            if (info.homePos.y == 1.25 && (info.homePos.x == 0 || info.homePos.z == 0))
            {
                if (!IsMiddleCube(info.id))
                {
                    //Debug.Log("TOP MIDDLE "+info.id);
                    result.Add(info);
                }
            }
        }
        return result;
    }

    public ArrayList FindTopCorners()
    {
        ArrayList result = new ArrayList();
        foreach (Cubie info in _cubeInfos)
        {
            if (info.homePos.y == 1.25 && info.homePos.x != 0 && info.homePos.z != 0)
            {
                result.Add(info);
            }
        }
        return result;
    }

    public ArrayList FindMiddleMiddle()
    {
        ArrayList result = new ArrayList();
        foreach (Cubie info in _cubeInfos)
        {
            if (info.homePos.y == 0 && info.homePos.x != 0 && info.homePos.z != 0)
            {
                result.Add(info);
            }
        }
        return result;
    }

    // Check for out of place top middle cubes and pick one to solve
    public void AnalyzeTopMiddle(ref CubeInfo.Cubie cubeToFix, ArrayList cubesOk)
    {
        cubeToFix = null;
        ArrayList topMiddle = FindTopMiddle();
        foreach (Cubie c in topMiddle)
        {
            if (c.transform.localPosition != c.homePos ||
                c.transform.localRotation.eulerAngles != c.homeRot)
            {
                cubeToFix = c;
            }
            else
            {
                cubesOk.Add(c);
            }
        }
    }

}

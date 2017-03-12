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
            _cubeInfos.Add(info);
        }
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
}

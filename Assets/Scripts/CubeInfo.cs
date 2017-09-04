﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeInfo 
{
    public const uint POS = 0x1;
    public const uint ORI = 0x2;
    public const uint TOP = 0x1;
    public const uint MID = 0x2;
    public const uint BOT = 0x4;

    public class Cubie
    {
        public int id;
        public Transform transform;
        public Vector3 homePos;
        public Vector3 homeRot;
        public int score; // for choosing best cubie to solve
        public uint state;
        public uint level;
    }
    private List<Cubie> _cubeInfos = new List<Cubie>();

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

    public void Reset()
    {
        foreach (Cubie c in _cubeInfos)
        {
            c.transform.localPosition = c.homePos;
            c.transform.localEulerAngles = c.homeRot;
        }
    }

    public bool IsSolved()
    {
        foreach (Cubie c in _cubeInfos)
        {
            if (!IsSolved(c))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsSolved(Cubie c)
    {
        return (CorrectOri(c) && CorrectPos(c));
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

    public bool IsCenterCube(int id)
    {
        Cubie info = _cubeInfos[id] as Cubie;

        int zeroCount = 0;
        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(info.homePos[i]) < 0.001f) zeroCount++;
        }

        return zeroCount == 2;
    }

    public List<Cubie> FindTopMiddle()
    {
        // top is Y = 1.25
        // middle is X or Z == 0
        List<Cubie> result = new List<Cubie>();
        foreach (Cubie info in _cubeInfos)
        {
            if (info.homePos.y == 1.25 && (info.homePos.x == 0 || info.homePos.z == 0))
            {
                if (!IsCenterCube(info.id))
                {
                    //Debug.Log("TOP MIDDLE "+info.id);
                    result.Add(info);
                }
            }
        }
        return result;
    }

    public List<Cubie> FindTopCorners()
    {
        List<Cubie> result = new List<Cubie>();
        foreach (Cubie info in _cubeInfos)
        {
            if (info.homePos.y == 1.25 && info.homePos.x != 0 && info.homePos.z != 0)
            {
                result.Add(info);
            }
        }
        return result;
    }

    public List<Cubie> FindBottomCorners()
    {
        List<Cubie> result = new List<Cubie>();
        foreach (Cubie info in _cubeInfos)
        {
            if (info.homePos.y == -1.25 && info.homePos.x != 0 && info.homePos.z != 0)
            {
                result.Add(info);
            }
        }
        return result;
    }

    public List<Cubie> FindMiddleMiddle()
    {
        List<Cubie> result = new List<Cubie>();
        foreach (Cubie info in _cubeInfos)
        {
            if (info.homePos.y == 0 && info.homePos.x != 0 && info.homePos.z != 0)
            {
                result.Add(info);
            }
        }
        return result;
    }

    public List<Cubie> FindBottomMiddle()
    {
        // top is Y = 1.25
        // middle is X or Z == 0
        List<Cubie> result = new List<Cubie>();
        foreach (Cubie info in _cubeInfos)
        {
            if (info.homePos.y == -1.25 && (info.homePos.x == 0 || info.homePos.z == 0))
            {
                if (!IsCenterCube(info.id))
                {
                    //Debug.Log("TOP MIDDLE "+info.id);
                    result.Add(info);
                }
            }
        }
        return result;
    }

    public bool Contains(List<Cubie> list, Cubie c)
    {
        foreach (Cubie cc in list)
        {
            if (cc == c) return true;
        }
        return false;
    }

    public bool CorrectOri(Cubie c)
    {
        Vector3 euler = c.transform.localEulerAngles;
        return (c.homeRot - euler).sqrMagnitude < 0.001f;
    }

    public bool CorrectPos(Cubie c)
    {
        return (c.homePos - c.transform.localPosition).sqrMagnitude < 0.001f;
    }

    public bool TopRow(Cubie c)
    {
        return Mathf.Abs(c.transform.localPosition.y - 1.25f) < 0.0001f;
    }

    public bool MiddleRow(Cubie c)
    {
        return Mathf.Abs(c.transform.localPosition.y) < 0.0001f;
    }

    public bool BottomRow(Cubie c)
    {
        return Mathf.Abs(c.transform.localPosition.y + 1.25f) < 0.0001f;
    }

    public bool FrontRight(Cubie c)
    {
        return Mathf.Abs(c.transform.localPosition.x - 1.25f) < 0.0001f &&
               Mathf.Abs(c.transform.localPosition.z - 1.25f) < 0.0001f;
    }

    public bool RightBack(Cubie c)
    {
        return Mathf.Abs(c.transform.localPosition.x + 1.25f) < 0.0001f &&
               Mathf.Abs(c.transform.localPosition.z - 1.25f) < 0.0001f;
    }

    public bool BackLeft(Cubie c)
    {
        return Mathf.Abs(c.transform.localPosition.x + 1.25f) < 0.0001f &&
               Mathf.Abs(c.transform.localPosition.z + 1.25f) < 0.0001f;
    }

    public bool LeftFront(Cubie c)
    {
        return Mathf.Abs(c.transform.localPosition.x - 1.25f) < 0.0001f &&
               Mathf.Abs(c.transform.localPosition.z + 1.25f) < 0.0001f;
    }

    public bool FacingDown(Cubie c)
    {
        Vector3 localUp = c.transform.TransformDirection(Vector3.up);
        Debug.Log(localUp);
        return Mathf.Abs(localUp.y + 1) < 0.0001f;
    }

    public bool FacingUp(Cubie c)
    {
        Vector3 localUp = c.transform.TransformDirection(Vector3.up);
        return Mathf.Abs(localUp.y - 1) < 0.0001f;
    }

    // Check for out of place top middle cubes and pick one to solve
    // Ignore the cubes in constraints, because we know these are solved already
    // Scoring assumes that top row is already solved
    public List<Cubie> AnalyzeMiddleMiddle(ref CubeInfo.Cubie cubeToFix, List<Cubie> constraints)
    {
        int bestScore = 0;
        cubeToFix = null;
        List<Cubie> middleMiddle = FindMiddleMiddle();
        foreach (Cubie c in middleMiddle)
        {
            c.score = 0;
            if (Contains(constraints, c)) continue;

            if (CorrectPos(c)) { c.score += 2; c.state = c.state | POS;  }
            if (CorrectOri(c)) { c.score += 2; c.state = c.state | ORI; }

            if (BottomRow(c)) { c.score += 2; c.level = BOT; }
            else if (MiddleRow(c)) { c.score += 1; c.level = MID; }

            if (c.score > bestScore )
            {
                bestScore = c.score;
                cubeToFix = c;
            }
        }
        return middleMiddle;
    }


    // Check for out of place top middle cubes and pick one to solve
    // Ignore the cubes in constraints, because we know these are solved already
    public List<Cubie> AnalyzeTopMiddle(ref CubeInfo.Cubie cubeToFix, List<Cubie> constraints)
    {
        int bestScore = 0;
        cubeToFix = null;
        List<Cubie> topMiddle = FindTopMiddle();
        foreach (Cubie c in topMiddle)
        {
            c.score = 0;
            if (Contains(constraints, c)) continue;

            if (CorrectPos(c)) { c.score += 2; c.state = c.state | POS;  }
            if (CorrectOri(c)) { c.score += 2; c.state = c.state | ORI; }

            if (TopRow(c)) { c.score += 2; c.level = TOP; }
            else if (MiddleRow(c)) { c.score += 1; c.level = MID; }
            else { c.score += 1; c.level = BOT; }

            if (c.score > bestScore )
            {
                bestScore = c.score;
                cubeToFix = c;
            }
        }
        return topMiddle;
    }

    // Check for out of place top middle cubes and pick one to solve
    // Ignore the cubes in constraints, because we know these are solved already
    public List<Cubie> AnalyzeTopCorner(ref CubeInfo.Cubie cubeToFix, List<Cubie> constraints)
    {
        int bestScore = 0;
        cubeToFix = null;
        List<Cubie> topCorner = FindTopCorners();
        foreach (Cubie c in topCorner)
        {
            c.score = 0;
            if (Contains(constraints, c)) continue;

            if (CorrectPos(c)) { c.score += 2; c.state = c.state | POS; }
            if (CorrectOri(c)) { c.score += 2; c.state = c.state | ORI; }

            if (TopRow(c)) { c.score += 2; c.level = TOP; }
            else { c.score += 1; c.level = BOT; }

            if (c.score > bestScore)
            {
                bestScore = c.score;
                cubeToFix = c;
            }
        }
        return topCorner;
    }

    public enum CornerCase
    {
        CORRECT_ORDER = 0,
        ONE_CORRECT = 1,
        TWO_CONSECUTIVE_CORRECT = 2,
        TWO_NON_CONSECUTIVE_CORRECT = 3
    };

    /*
    public List<Cubie> AnalyzeBottomCorners(
        ref List<Cubie> cubesToFix, ref CornerCase sitch, List<Cubie> constraints)
    {
        int bestScore = 0;
        List<Cubie> bottomCorner = FindBottomCorners();
        CubeInfo.Cubie RB = bottomCorner.
        
        foreach (Cubie c in bottomCorner)
        {
            c.score = 0;
            if (Contains(constraints, c)) continue;

            if (CorrectPos(c)) { c.score += 2; c.state = c.state | POS; }
            if (CorrectOri(c)) { c.score += 2; c.state = c.state | ORI; }

            if (TopRow(c)) { c.score += 2; c.level = TOP; }
            else { c.score += 1; c.level = BOT; }

            if (c.score > bestScore)
            {
                bestScore = c.score;
                cubeToFix = c;
            }
        }
        return bottomCorner;

    }*/

}

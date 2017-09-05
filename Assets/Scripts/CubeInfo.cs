using UnityEngine;
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


    public bool HomeIsRightBack(Cubie c)
    {
        return Mathf.Abs(c.homePos.x + 1.25f) < 0.0001f &&
               Mathf.Abs(c.homePos.z - 1.25f) < 0.0001f;
    }

    public bool HomeIsBackLeft(Cubie c)
    {
        return Mathf.Abs(c.homePos.x + 1.25f) < 0.0001f &&
               Mathf.Abs(c.homePos.z + 1.25f) < 0.0001f;
    }

    public bool HomeIsLeftFront(Cubie c)
    {
        return Mathf.Abs(c.homePos.x - 1.25f) < 0.0001f &&
               Mathf.Abs(c.homePos.z + 1.25f) < 0.0001f;
    }

    public bool HomeIsFrontRight(Cubie c)
    {
        return Mathf.Abs(c.homePos.x - 1.25f) < 0.0001f &&
               Mathf.Abs(c.homePos.z - 1.25f) < 0.0001f;
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

    bool CorrectCornerOrder(List<Cubie> order) // clockwise listing from RB
    {
        int RBidx = -1;
        for (int i = 0; i < order.Count && RBidx == -1; i++)
        {
            if (HomeIsRightBack(order[i])) RBidx = i;
        }

        int BLidx = (RBidx + 1) % order.Count;
        int LFidx = (RBidx + 2) % order.Count;
        int FRidx = (RBidx + 3) % order.Count;

        bool check1 = HomeIsBackLeft(order[BLidx]);
        bool check2 = HomeIsLeftFront(order[LFidx]);
        bool check3 = HomeIsFrontRight(order[FRidx]);

        return check1 && check2 && check3;
    }

    // return true if the bottom corners are in the correct order
    public bool BottomCornersSorted()
    {
        List<Cubie> bottomCorner = FindBottomCorners();
        CubeInfo.Cubie RB = bottomCorner.Find(x => RightBack(x));
        CubeInfo.Cubie BL = bottomCorner.Find(x => BackLeft(x));
        CubeInfo.Cubie LF = bottomCorner.Find(x => LeftFront(x));
        CubeInfo.Cubie FR = bottomCorner.Find(x => FrontRight(x));

        List<Cubie> clockwise = new List<Cubie>(4);
        clockwise.Add(RB);
        clockwise.Add(BL);
        clockwise.Add(LF);
        clockwise.Add(FR);
        
        bool sorted = CorrectCornerOrder(clockwise);
        return sorted;
    }

    public bool BottomCornersCorrectPositions()
    {
        List<Cubie> bottom = FindBottomCorners();
        foreach (Cubie c in bottom)
        {
            if (!CorrectPos(c)) return false;
        }
        return true;
    }

    public bool BottomMiddlesCorrectPositions()
    {
        List<Cubie> bottom = FindBottomMiddle();
        foreach (Cubie c in bottom)
        {
            if (!CorrectPos(c)) return false;
        }
        return true;
    }

    public bool TopMiddleSolved()
    {
        List<Cubie> topMiddle = FindTopMiddle();
        foreach (Cubie c in topMiddle)
        {
            if (!IsSolved(c)) return false;
        }
        return true;
    }

    public bool TopCornersSolved()
    {
        List<Cubie> top = FindTopCorners();
        foreach (Cubie c in top)
        {
            if (!IsSolved(c)) return false;
        }
        return true;
    }

    public bool MiddleMiddleSolved()
    {
        List<Cubie> top = FindMiddleMiddle();
        foreach (Cubie c in top)
        {
            if (!IsSolved(c)) return false;
        }
        return true;
    }

    // find a top corner with the correct position to make as a reference point
    public List<Cubie> AnalyzeBottomCorners(ref Cubie cubie, List<Cubie> constraints)
    {
        // TODO: Could search for consecutive pair too
        List<Cubie> bottom = FindBottomCorners();
        cubie = bottom[0]; // just use first one by default
        foreach (Cubie c in bottom)
        {
            if (FacingUp(c)) // facing up means facing correct way on bottom row
            {
                cubie = c;
                break;
            }
        }
        return bottom;
    }

    // return true if at least one corner is correct
    public bool BottomOneCornerCorrect()
    {
        List<Cubie> bottom = FindBottomCorners();
        foreach (Cubie c in bottom)
        {
            if (CorrectPos(c)) return true;
        }
        return false;
    }
}

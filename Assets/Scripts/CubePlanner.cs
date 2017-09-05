using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CubePlanner : MonoBehaviour
{
    public string startState = "F' D";

    private CubeStateManager _cube = new CubeStateManager();
    private CubeInfo _cubies = new CubeInfo();
    private CubeTaskSolver _solver = new CubeTaskSolver();

    private int _currentTask = 0;
    private List<CubeTask> _tasks = new List<CubeTask>();
    private List<CubeInfo.Cubie> _solved = new List<CubeInfo.Cubie>();
    
    void Start()
    {
        _cube.init(transform);
        _cubies.init(transform); 
        _solver.init(_cubies, _cube);

        if (startState == "Random")
        {
            string[] choices = { "F", "F", "F2",
                                 "B", "B", "B2",
                                 "R", "R", "R2",
                                 "L", "L", "L2",
                                 "U", "U", "U2",
                                 "D", "D", "D2"};
            int maxMoves = 10;
            System.Random random = new System.Random();
            for (int i = 0; i < maxMoves; i++)
            {
                int idx = random.Next(0, choices.Length);
                string cmd = choices[idx];
                _cube.turn(cmd);
                Debug.Log(cmd);
            }
        }
        else
        {
            char[] delimiterChars = { ' ' };
            string[] words = startState.Split(delimiterChars);
            for (int w = 0; w < words.Length; w++)
            {
                string word = words[w];
                _cube.turn(word);
            }
        }

        _tasks.Add(new CubeTask(SolveTopMiddle, _cubies.TopMiddleSolved));
        _tasks.Add(new CubeTask(SolveTopCorners, _cubies.TopCornersSolved));
        _tasks.Add(new CubeTask(SolveMiddleMiddles, _cubies.MiddleMiddleSolved));
        _tasks.Add(new CubeTask(SolveOneCornerPosition, _cubies.BottomOneCornerCorrect));
        _tasks.Add(new CubeTask(SolveBottomCornerPositions, _cubies.BottomCornersCorrectPositions));
        _tasks.Add(new CubeTask(SolveBottomMiddlePositions, _cubies.BottomMiddlesCorrectPositions));
        _tasks.Add(new CubeTask(SolveBottomCornerOri, _cubies.BottomMiddlesCorrectOri));
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //List<string> path = new List<string>();
            //SolveOneCornerPosition(ref path);
            //ExecutePath(path); // changes planning cube immediately
            //Debug.Log("TEST " + PathToString(path)+" "+ _cubies.BottomOneCornerCorrect());

            //List<string> path = new List<string>();
            //SolveBottomMiddlePositions(ref path);
            //ExecutePath(path); // changes planning cube immediately
            //Debug.Log("TEST " + PathToString(path)+" "+ _cubies.BottomMiddlesCorrectPositions());

            List<string> path = new List<string>();
            SolveBottomCornerOri(ref path);
            ExecutePath(path); // changes planning cube immediately
            Debug.Log("TEST " + PathToString(path)+" "+ _cubies.BottomCornersCorrectOri());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Test("B' D B R D' R' D2"); // Run Test1: D R D2 R'  B' D' B

            // Test analyzing the bottom face of cubes
            //List<CubeInfo.Cubie> bottomCornerCubes = _cubies.AnalyzeBottomCorners( ref sorted);
            //List<string> path = new List<string>();
            //SolveBottomCornerPositions(ref path);
            //ExecutePath(path); // changes planning cube immediately
            //Debug.Log("TEST " + PathToString(path)+" "+ _cubies.BottomCornersCorrectPositions());

            //UpdateCubeState(); // only want to do this once in the beginning
            // in the future, we don't need a shadow cube
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StepTask();
        }
	}

    void StepTask()
    {
        List<string> path = new List<string>();
        SolveTask(ref path);

        //AnimatePath(path); // for visuals, changes display cube over many frames
        // apply state changes and get ready for next iteration
        Debug.Log("PATH " + PathToString(path));
        ExecutePath(path); // changes planning cube immediately
    }

    void SolveTask(ref List<string> path)
    {
        CubeTask task = _tasks[_currentTask] as CubeTask;
        if (task.Finished() && _currentTask+1 < _tasks.Count)
        {
            _currentTask++;
        }

        bool success = task.Solve(ref path);
        Debug.Log("Solve task: " + _currentTask + " "+ success);
    }

    bool SolveOneCornerPosition(ref List<string> path)
    {
        CubeInfo.Cubie cubie = null;
        List<CubeInfo.Cubie> bottom = _cubies.AnalyzeBottomCorners(ref cubie, _solved);
        if (cubie == null) return true; // no work to do

        Debug.Log("FIX POS " + cubie.id);

        string[] down = { "D", "D'", "D2", "" };
        ArrayList steps = new ArrayList();
        steps.Add(down);
        path = _solver.Search(cubie, _solved, steps, _solver.ScorePositions);

        return path.Count > 0;
    }

    bool SolveBottomCornerOri(ref List<string> path)
    {
        CubeInfo.Cubie cubie = null;
        List<CubeInfo.Cubie> bottom = _cubies.AnalyzeBottomCornerOri(ref cubie, _solved);
        if (cubie == null) return true; // no work to do

        Debug.Log("FIX ORI: " + cubie.id);

        string[] down = { "D", "D'", "D2", "" };
        string[] seqnA = 
        {
            "B' U B L U L'",
            "R' U R B U B'",
            "F' U F R U R'",
            "L' U L F U F'"
        };
        string[] seqnB =
        {
            "L U' L' B' U' B",
            "B U' B' R' U' R",
            "R U' R' F' U' F",
            "F U' F' L' U' L"
        };

        ArrayList steps = new ArrayList();
        for (int i = 0; i < seqnA.Length; i++)
        {
            steps.Clear();
            string[] words = seqnA[i].Split();
            foreach (string word in words)
            {
                string[] tmp = { word };
                steps.Add(tmp);
            }
            steps.Add(down);
            words = seqnB[i].Split();
            foreach (string word in words)
            {
                string[] tmp = { word };
                steps.Add(tmp);
            }
            steps.Add(down);
            path = _solver.Search(cubie, _solved, steps, _solver.ScoreSolved);
            if (path.Count > 0) break;
        }

        return path.Count > 0;
    }

    bool SolveBottomMiddlePositions(ref List<string> path)
    {
        List<CubeInfo.Cubie> bottom = _cubies.FindBottomMiddle();
        bool sorted = _cubies.BottomMiddlesCorrectPositions();
        if (sorted) return true; // no work to do

        // case 1: need to rotate three
        // case 2: need to swap consecutive cubies
        // case 3: need to swap opposite cubies
        ArrayList steps = new ArrayList();
        string[] sequences =
        {
            //rotate C
            "F2 D L' R F2 L R' D F2", 
            "L2 D B' F L2 B F' D L2", 
            "B2 D R' L B2 R L' D B2", 
            "R2 D F' B R2 F B' D R2", 

            //rotate CC
            "F2 D' L' R F2 L R' D' F2", 
            "L2 D' B' F L2 B F' D' L2", 
            "B2 D' R' L B2 R L' D' B2", 
            "R2 D' F' B R2 F B' D' R2", 

            //swap opposite
            "R2 L2 U R2 L2 D2 R2 L2 U R2 L2", 
            "R F D F' D' R2 B' D' B D R", 

            //swap neighbors
            "R F D F' D' R2 B' D' B D R", 
            "B R D R' D' B2 L' D' L D B" 
        };

        // ASN TODO: We know the sequence if we analyze the cube more closely
        foreach (string seqn in sequences)
        {
            steps.Clear();
            string[] words = seqn.Split();
            foreach (string word in words)
            {
                string[] tmp = { word };
                steps.Add(tmp);
            }
            path = _solver.Search(null, _solved, steps, _solver.ScorePositions);
            if (path.Count > 0) break;
        }

        return path.Count > 0;
    }

    bool SolveBottomCornerPositions(ref List<string> path)
    {
        List<CubeInfo.Cubie> bottomCorners = _cubies.FindBottomCorners();
        bool sorted = _cubies.BottomCornersSorted();

        // case 1: sorted but possible needs a rotation
        // case 2: need to rotate three
        // case 3: need to swap consecutive cubies
        // case 4: need to swap opposite cubies
        if (sorted)
        {
            string[] down = { "D", "D'", "D2", "" };
            ArrayList steps = new ArrayList();
            steps.Add(down);
            path = _solver.Search(null, _solved, steps, _solver.ScorePositions);
        }
        else
        {
            ArrayList steps = new ArrayList();
            string[] down = { "D", "D'", "D2", "" };
            string[] sequences =
            {
                // rotate C
                "L' D R D' L D R' D'", 
                "F' D B D' F D B' D'",
                "R' D L D' R D L' D'",
                "B' D F D' B D F' D'",

                // rotate CC
                "D R D' L' D R' D' L",
                "D B D' F' D B' D' F",
                "D L D' R' D L' D' R",
                "D F D' B' D F' D' B",

                // swap consecutive
                "F D' B' D F' D' B D2",
                "B D' F' D B' D' F D2",
                "L D' R' D L' D' R D2",
                "R D' L' D R' D' L D2",

                // swap non-consecutive
                "D F D L D' L' F'",
                "D R D F D' F' R'",
                "D B D R D' R' B'",
                "D L D B D' B' L'"
            };

            // ASN TODO: We know the sequence if we analyze the cube more closely
            foreach (string seqn in sequences)
            {
                steps.Clear();
                //steps.Add(down);
                string[] words = seqn.Split();
                foreach (string word in words)
                {
                    string[] tmp = { word };
                    steps.Add(tmp);
                }
                path = _solver.Search(null, _solved, steps, _solver.ScorePositions);
                if (path.Count > 0) break;
            }
        }

        return path.Count > 0;
    }

    bool SolveMiddleMiddles(ref List<string> path)
    {
        CubeInfo.Cubie cubie = null;
        List<CubeInfo.Cubie> middleMiddleCubes = _cubies.AnalyzeMiddleMiddle(ref cubie, _solved);
        if (cubie == null) return true; // no work to do!

        Debug.Log("FIX " + cubie.id);

        // case 1: no work to do -> no work to do, return
        // case 2: cube is in top row, but wrong pos or ori
        // case 3: cube is in middle row
        // case 4: cube is in bottom row

        if (cubie.state == (CubeInfo.POS | CubeInfo.ORI))
        {
            _solved.Add(cubie);
            return _solved.Count >= 8; // this hard-codes the order of steps (middle first => 4 cubes, corner next => 8 cubes)
        }

        if (cubie.level == CubeInfo.BOT)
        {
            return SolveMiddleMiddle_CaseBottom(cubie, ref path);
        }
        else
        {
            Debug.Log("MiddleMiddle cubie right position; wrong ori -> moving to bottom");
            return SolveMiddleMiddle_CaseMiddle(cubie, ref path); // move to bottom and then solve
        }
    }

    bool SolveMiddleMiddle_CaseMiddle(CubeInfo.Cubie cubie, ref List<string> path)
    {
        if (_cubies.FrontRight(cubie))
        {
            string[] seqn = { "F", "D'", "F'", "D'", "R'", "D", "R"};
            path.AddRange(seqn);
        }
        else if (_cubies.RightBack(cubie))
        {
            string[] seqn = { "R", "D'", "R'", "D'", "B'", "D", "B" };
            path.AddRange(seqn);
        }
        else if (_cubies.BackLeft(cubie))
        {
            string[] seqn = { "B", "D'", "B'", "D'", "L'", "D", "L" };
            path.AddRange(seqn);
        }
        else if (_cubies.LeftFront(cubie))
        {
            string[] seqn = { "L", "D'", "L'", "D'", "F'", "D", "F" };
            path.AddRange(seqn);
        }
        return false;
    }

    bool SolveMiddleMiddle_CaseBottom(CubeInfo.Cubie cubie, ref List<string> path)
    {
        ArrayList steps = new ArrayList();
        string[] down = { "D", "D'", "D2", "" };
        string[] sequences = 
        {
            "D' L' D L D B D' B'",
            "D' R' D R D F D' F'",
            "D' B' D B D R D' R'",
            "D' F' D F D L D' L'",
            "D R D' R' D' B' D B",
            "D F D' F' D' R' D R",
            "D L D' L' D' F' D F",
            "D B D' B' D' L' D L"
        };

        // ASN TODO: We know the sequence if we analyze the cube more closely
        foreach (string seqn in sequences)
        {
            steps.Clear();
            steps.Add(down);
            string[] words = seqn.Split();
            foreach (string word in words)
            {
                string[] tmp = { word };
                steps.Add(tmp);
            }
            path = _solver.Search(cubie, _solved, steps, _solver.ScoreSolved);
            if (path.Count > 0) break;
        }

        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 12;
    }

    bool SolveTopCorners(ref List<string> path)
    {
        CubeInfo.Cubie cubie = null;
        List<CubeInfo.Cubie> topCornerCubes = _cubies.AnalyzeTopCorner(ref cubie, _solved);
        if (cubie == null) return true; // no work to do!

        Debug.Log("FIX " + cubie.id);

        // case 1: no work to do -> no work to do, return
        // case 2: cube is in top row, but wrong pos or ori
        // case 3: cube is in middle row
        // case 4: cube is in bottom row

        if (cubie.state == (CubeInfo.POS | CubeInfo.ORI))
        {
            _solved.Add(cubie);
            return _solved.Count >= 8; // this hard-codes the order of steps (middle first => 4 cubes, corner next => 8 cubes)
        }

        if (cubie.level == CubeInfo.TOP)
        {
            return SolveTopCorner_CaseTop(cubie, ref path);
        }
        else
        {
            return SolveTopCorner_CaseBottom(cubie, ref path);
        }
    }

    bool SolveTopCorner_CaseTop(CubeInfo.Cubie cubie, ref List<string> path)
    {
        ArrayList steps = new ArrayList();
        string[] sides = {"L", "L'",
                           "R", "R'",
                           "F", "F'",
                           "B", "B'"};
        string[] down = { "D", "D'", "D2", "" };
        string[] D2 = { "D2" };
        string[] reverse = { "X" };
        steps.Add(sides);
        steps.Add(D2);
        steps.Add(reverse); // reverse level0

        steps.Add(down);
        steps.Add(sides);
        steps.Add(down);
        steps.Add(reverse); // reverse level0

        path = _solver.Search(cubie, _solved, steps, _solver.ScoreSolved);
        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 8;
    }

    bool SolveTopCorner_CaseBottom(CubeInfo.Cubie cubie, ref List<string> path)
    {
        ArrayList steps = new ArrayList();
        string[] down = { "D", "D'", "D2", "" };
        string[] sides = {"L", "L'", 
                           "R", "R'", 
                           "F", "F'", 
                           "B", "B'"};
        string[] D2 = { "D2" };
        string[] reverse = { "X" };

        if (_cubies.FacingDown(cubie))
        {
            Debug.Log("FACING DOWN");
            steps.Add(down);
            steps.Add(sides);
            steps.Add(D2);
            steps.Add(reverse);
        }

        steps.Add(down);
        steps.Add(sides);
        steps.Add(down);
        steps.Add(reverse);

        path = _solver.Search(cubie, _solved, steps, _solver.ScoreSolved);
        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 8;
    }

    bool SolveTopMiddle(ref List<string> path)
    {
        CubeInfo.Cubie cubie = null;
        List<CubeInfo.Cubie> topMiddleCubes = _cubies.AnalyzeTopMiddle(ref cubie, _solved);
        if (cubie == null) return true; // no work to do!

        Debug.Log("FIX " + cubie.id);

        // case 1: no work to do -> no work to do, return
        // case 2: cube is in top row, but wrong pos or ori
        // case 3: cube is in middle row
        // case 4: cube is in bottom row

        if (cubie.state == (CubeInfo.POS | CubeInfo.ORI))
        {
            _solved.Add(cubie);
            return _solved.Count >= 4;
        }

        if (cubie.level == CubeInfo.TOP)
        {
            return SolveTopMiddle_CaseTop(cubie, ref path);
        }
        else if (cubie.level == CubeInfo.MID)
        {
            return SolveTopMiddle_CaseMiddle(cubie, ref path);
        }
        else
        {
            return SolveTopMiddle_CaseBottom(cubie, ref path);
        }
    }

    bool SolveTopMiddle_CaseTop(CubeInfo.Cubie cubie, ref List<string> path)
    {
        ArrayList steps = new ArrayList();
        string[] level0 = { "U", "U'", "U2", "" };
        string[] level1 = {"L", "L'", 
                           "R", "R'", 
                           "F", "F'", 
                           "B", "B'", ""};
        steps.Add(level0);
        steps.Add(level1);
        steps.Add(level0);
        steps.Add(level1);
        steps.Add(level0);

        path = _solver.Search(cubie, _solved, steps, _solver.ScoreSolved);
        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 4;
    }

    bool SolveTopMiddle_CaseMiddle(CubeInfo.Cubie cubie, ref List<string> path)
    { 
        ArrayList steps = new ArrayList();
        string[] level0 = { "U", "U'", "U2", "" };
        string[] level1 = {"L", "L'", 
                           "R", "R'", 
                           "F", "F'", 
                           "B", "B'"};
        steps.Add(level0);
        steps.Add(level1);
        steps.Add(level0);

        path = _solver.Search(cubie, _solved, steps, _solver.ScoreSolved);
        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 4;
    }

    bool SolveTopMiddle_CaseBottom(CubeInfo.Cubie cubie, ref List<string> path)
    {
        ArrayList steps = new ArrayList();

        string[] level0 = { "D", "D'", "D2", "" };
        string[] level10 = { "L2", "R2", "F2", "B2" }; 
        steps.Add(level0);
        steps.Add(level10);

        path = _solver.Search(cubie, _solved, steps, _solver.ScoreSolved);

        if (path.Count == 0) // try other case
        {
            string[] level11 = {"L", "L'",
                            "R", "R'",
                            "F", "F'",
                            "B", "B'"};
            string[] level2 = { "U", "U'", "U2", "" };

            string[] level3 = {"L", "L'",
                            "R", "R'",
                            "F", "F'",
                            "B", "B'", ""};
            steps.Clear();
            steps.Add(level0);
            steps.Add(level11);
            steps.Add(level2);
            steps.Add(level3);
            steps.Add(level3);
            path = _solver.Search(cubie, _solved, steps, _solver.ScoreSolved);
        }

        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 4;
    }

    void AnimatePath(List<string> path)
    {
        CubeDemo demo = gameObject.GetComponent<CubeDemo>();
        if (demo == null)
        {
            Debug.LogWarning("No CubeDemo component. Cannot animate!");
            return; // not animating
        }

        string s = "";
        for (int i = 0; i < path.Count; i++)
        {
            if ((string) path[i] == "") continue;
            s += path[i] + " ";
        }
        Debug.Log("FOUND " + s);

        demo.ParseRecipe(s);
    }

    void ExecutePath(List<string> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if ((string) path[i] == "") continue;
            _cube.turn((string) path[i]);
        }
    }

    string PathToString(List<string> path)
    {
        string ss = "";
        for (int i = 0; i < path.Count; i++)
        {
            ss += " " + path[i];
        }
        ss = ss.Trim();
        return ss;
    }

    void Test1()
    {
        _cubies.Reset();
        _cube.turn("F'");
        _cube.turn("D");

        CubeInfo.Cubie cubie = null;
        List<CubeInfo.Cubie> list = _cubies.FindTopMiddle();
        foreach (CubeInfo.Cubie c in list)
        {
            if (_cubies.MiddleRow(c))
            {
                cubie = c;
                break;
            }
        }

        List<string> path = new List<string>();
        SolveTopMiddle_CaseMiddle(cubie, ref path);

        string s = PathToString(path);
        Debug.Log("Run Test1: " + s);
        Debug.Assert(s == "F");
    }
    void Test2()
    {
        _cubies.Reset();
        _cube.turn("F2");

        CubeInfo.Cubie cubie = null;
        List<CubeInfo.Cubie> list = _cubies.FindTopMiddle();
        foreach (CubeInfo.Cubie c in list)
        {
            if (_cubies.BottomRow(c))
            {
                cubie = c;
                break;
            }
        }

        List<string> path = new List<string>();
        SolveTopMiddle_CaseBottom(cubie, ref path);

        string s = PathToString(path);
        Debug.Log("Run Test1: " + s);
        Debug.Assert(s == "F2");
    }

    void Test(string cmd)
    {
        _cubies.Reset();
        _cube.SortCubeGroups();
        string[] words = cmd.Split();
        foreach (string word in words)
        {
            _cube.turn(word);
        }

        CubeInfo.Cubie cubie = null;
        List<CubeInfo.Cubie> list = _cubies.FindTopCorners();
        foreach (CubeInfo.Cubie c in list)
        {
            if (_cubies.BottomRow(c))
            {
                cubie = c;
                break;
            }
        }
        Debug.Assert(cubie != null);

        List<string> path = new List<string>();
        SolveTopCorner_CaseBottom(cubie, ref path);

        string s = PathToString(path);
        Debug.Log("Run Test1: " + s);
        words = s.Split();
        foreach (string word in words)
        {
            _cube.turn(word);
        }
        Debug.Assert(_cubies.IsSolved(cubie));
        //Debug.Assert(s == "D' R D R");
    }

    void Test3()
    {
        // F' D F L D L' (twisted top corner cube)
    }

    void Test4()
    {
        // R' D' L D L' R (test two corner cubes out of place)
    }
    //F D2 F' D -> D' F D2 F'
}

/*
Corners:
rotate C
L' D R D' L D R' D'
F' D B D' F D B' D'
R' D L D' R D L' D'
B' D F D' B D F' D'

rotate CC
D R D' L' D R' D' L
D B D' F' D B' D' F
D L D' R' D L' D' R
D F D' B' D F' D' B

swap consecutive
F D' B' D F' D' B D2
B D' F' D B' D' F D2
L D' R' D L' D' R D2
R D' L' D R' D' L D2

swap non-consecutive
D F D L D' L' F'
D R D F D' F' R'
D B D R D' R' B'
D L D B D' B' L'

Middle:
rotate C
F2 D L' R F2 L R' D F2
L2 D B' F L2 B F' D L2
B2 D R' L B2 R L' D B2
R2 D F' B R2 F B' D R2

rotate CC
F2 D' L' R F2 L R' D' F2
L2 D' B' F L2 B F' D' L2
B2 D' R' L B2 R L' D' B2
R2 D' F' B R2 F B' D' R2

swap opposite
R2 L2 U R2 L2 D2 R2 L2 U R2 L2
R F D F' D' R2 B' D' B D R

swap neighbors
R F D F' D' R2 B' D' B D R
B R D R' D' B2 L' D' L D B

Orientation:

corner flip:
B' U B L U L' {D} L U' L' B' U' B {D'}
R' U R B U B' {D} B U' B' R' U' R {D'}
F' U F R U R' {D} R U' R' F' U' F {D'}
L' U L F U F' {D} F U' F' L' U' L {D'}

middle flip:
F U' D R2 U2 D2 L {D} L' D2 U2 R2 D' U F' {D'}
L U' D F2 U2 D2 B {D} B' D2 U2 F2 D' U L' {D'}
B U' D L2 U2 D2 R {D} R' D2 U2 L2 D' U B' {D'}
R U' D B2 U2 D2 F {D} F' D2 U2 B2 D' U R' {D'}

    */
using UnityEngine;
using System.Collections;
using System;

public class CubePlanner : MonoBehaviour
{
    public string startState = "F' D";

    private CubeStateManager _cube = new CubeStateManager();
    private CubeInfo _cubies = new CubeInfo();
    private CubeTaskSolver _solver = new CubeTaskSolver();

    private ArrayList _tasks = new ArrayList();
    private int _currentTask = 0;
    private ArrayList _solved = new ArrayList();
    
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

        _tasks.Add(new CubeTask(SolveTopMiddle));
        _tasks.Add(new CubeTask(SolveTopCorners));
    }


    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Test("B' D B R D' R' D2"); // Run Test1: D R D2 R'  B' D' B

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
        ArrayList path = new ArrayList();
        SolveTask(ref path);

        //AnimatePath(path); // for visuals, changes display cube over many frames
        // apply state changes and get ready for next iteration
        Debug.Log("PATH " + PathToString(path));
        ExecutePath(path); // changes planning cube immediately
    }

    void SolveTask(ref ArrayList path)
    {
        Debug.Log("Solve task: " + _currentTask);
        CubeTask task = _tasks[_currentTask] as CubeTask;
        bool finished = task.Solve(ref path);
        if (finished && _currentTask+1 < _tasks.Count)
        {
            _currentTask++;
        }
    }

    bool SolveTopCorners(ref ArrayList path)
    {
        CubeInfo.Cubie cubie = null;
        ArrayList topCornerCubes = _cubies.AnalyzeTopCorner(ref cubie, _solved);
        if (cubie == null) return true; // no work to do!

        Debug.Log("FIX " + cubie.id);

        // case 1: no work to do -> no work to do, return
        // case 2: cube is in top row, but wrong pos or ori
        // case 3: cube is in middle row
        // case 4: cube is in bottom row

        if (cubie.state == (CubeInfo.POS | CubeInfo.ORI))
        {
            _solved.Add(cubie);
            Debug.Log("NO WORK TO DO");
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

    bool SolveTopCorner_CaseTop(CubeInfo.Cubie cubie, ref ArrayList path)
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

        path = _solver.Search(cubie, _solved, steps);
        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 8;
    }

    bool SolveTopCorner_CaseBottom(CubeInfo.Cubie cubie, ref ArrayList path)
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

        path = _solver.Search(cubie, _solved, steps);
        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 8;
    }

    bool SolveTopMiddle(ref ArrayList path)
    {
        CubeInfo.Cubie cubie = null;
        ArrayList topMiddleCubes = _cubies.AnalyzeTopMiddle(ref cubie, _solved);
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

    bool SolveTopMiddle_CaseTop(CubeInfo.Cubie cubie, ref ArrayList path)
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

        path = _solver.Search(cubie, _solved, steps);
        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 4;
    }

    bool SolveTopMiddle_CaseMiddle(CubeInfo.Cubie cubie, ref ArrayList path)
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

        path = _solver.Search(cubie, _solved, steps);
        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 4;
    }

    bool SolveTopMiddle_CaseBottom(CubeInfo.Cubie cubie, ref ArrayList path)
    {
        ArrayList steps = new ArrayList();

        string[] level0 = { "D", "D'", "D2", "" };
        string[] level10 = { "L2", "R2", "F2", "B2" }; 
        steps.Add(level0);
        steps.Add(level10);

        path = _solver.Search(cubie, _solved, steps);

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
            path = _solver.Search(cubie, _solved, steps);
        }

        if (path.Count != 0) _solved.Add(cubie);
        return _solved.Count >= 4;
    }

    void AnimatePath(ArrayList path)
    {
        CubeDemo demo = gameObject.GetComponent<CubeDemo>();
        if (demo == null) return; // not animating

        string s = "";
        for (int i = 0; i < path.Count; i++)
        {
            if ((string) path[i] == "") continue;
            s += path[i] + " ";
        }
        Debug.Log("FOUND " + s);

        demo.ParseRecipe(s);
    }

    void ExecutePath(ArrayList path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if ((string) path[i] == "") continue;
            _cube.turn((string) path[i]);
        }
    }

    string PathToString(ArrayList path)
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
        ArrayList list = _cubies.FindTopMiddle();
        foreach (CubeInfo.Cubie c in list)
        {
            if (_cubies.MiddleRow(c))
            {
                cubie = c;
                break;
            }
        }

        ArrayList path = new ArrayList();
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
        ArrayList list = _cubies.FindTopMiddle();
        foreach (CubeInfo.Cubie c in list)
        {
            if (_cubies.BottomRow(c))
            {
                cubie = c;
                break;
            }
        }

        ArrayList path = new ArrayList();
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
        ArrayList list = _cubies.FindTopCorners();
        foreach (CubeInfo.Cubie c in list)
        {
            if (_cubies.BottomRow(c))
            {
                cubie = c;
                break;
            }
        }
        Debug.Assert(cubie != null);

        ArrayList path = new ArrayList();
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

using UnityEngine;
using System.Collections;
using System;

public class CubePlanner : MonoBehaviour {

    public GameObject scrap = null; // root of shadow cube who'se state mirrors the real cube; used for planning
    public string startState = "F' D";

    private ArrayList _shadowCube = new ArrayList();
    private CubeStateManager _cube = new CubeStateManager();
    private CubeInfo _cubies = new CubeInfo();
    private CubeTaskSolver _solver = new CubeTaskSolver();

    private ArrayList _tasks = new ArrayList();
    private int _currentTask = 0;
    private ArrayList _solved = new ArrayList();
    
    void Start()
    {
        CubeStateManager tmp = new CubeStateManager();
        tmp.init(scrap.transform); // hack: use init of statemgr to initialize exact cube positions of scrap

        _cubies.init(scrap.transform); 
        for (int i = 0; i < _cubies.GetNumCubes(); i++)
        {
            //GameObject obj = GameObject.Instantiate(scrap);
            GameObject obj = scrap.transform.GetChild(i).gameObject;

            CubeInfo.Cubie info = _cubies.GetCubeInfo(i);
            obj.transform.localPosition = info.transform.localPosition;
            obj.transform.localRotation = info.transform.localRotation;
            _shadowCube.Add(obj.transform);
        }

        //foreach (Transform t in _shadowCube)
        //{
            //t.parent = scrap.transform;
        //}

        _cube.init(scrap.transform);
        _solver.init(_cubies, _cube, _shadowCube);

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

        CubeTask task = new CubeTask(SolveTopMiddle);
        _tasks.Add(task);
    }


    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Test1();
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
        CubeTask task = _tasks[_currentTask] as CubeTask;
        bool finished = task.Solve(ref path);
        if (finished && _currentTask+1 < _tasks.Count)
        {
            _currentTask++;
            task = _tasks[_currentTask] as CubeTask;
        }
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
            return _solved.Count < 4;
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

        _solved.Add(cubie);
        path = _solver.Search(_solved, steps);
        return _solved.Count < 4;
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

        _solved.Add(cubie);
        path = _solver.Search(_solved, steps);
        return _solved.Count < 4;
    }

    bool SolveTopMiddle_CaseBottom(CubeInfo.Cubie cubie, ref ArrayList path)
    {
        ArrayList steps = new ArrayList();
        string[] level0 = { "D", "D'", "D2", "" };
        string[] level10 = { "L2", "R2", "F2", "B2" }; 
        steps.Add(level0);
        steps.Add(level10);

        _solved.Add(cubie);
        path = _solver.Search(_solved, steps);

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
                            "B", "B'"};
            steps.Clear();
            steps.Add(level0);
            steps.Add(level11);
            steps.Add(level2);
            steps.Add(level3);
            path = _solver.Search(_solved, steps);
        }

        return _solved.Count < 4;
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

    void UpdateCubeState()
    {
        for (int i = 0; i < _shadowCube.Count; i++)
        {
            Transform t = _shadowCube[i] as Transform;
            Transform infot = gameObject.transform.GetChild(i);
            t.localPosition = infot.localPosition;
            t.localRotation = infot.localRotation;
        }
        _cube.SortCubeGroups();
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
}

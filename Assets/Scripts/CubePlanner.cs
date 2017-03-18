using UnityEngine;
using System.Collections;

public class CubePlanner : MonoBehaviour {

    public GameObject scrap = null; // root of shadow cube who'se state mirrors the real cube; used for planning
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

        CubeTask task = new CubeTask(SolveTopMiddle);
        _tasks.Add(task);
    }


    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateCubeState(); // only want to do this once in the beginning
            // in the future, we don't need a shadow cube
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Solve();
        }
	}

    void Solve()
    {
        CubeTask task = _tasks[_currentTask] as CubeTask;
        bool finished = task.Solve();
        if (finished && _currentTask+1 < _tasks.Count)
        {
            _currentTask++;
            task = _tasks[_currentTask] as CubeTask;
        }
    }

    bool SolveTopMiddle()
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
            return SolveTopMiddle_CaseTop(cubie);
        }
        else if (cubie.level == CubeInfo.MID)
        {
            return SolveTopMiddle_CaseMiddle(cubie);
        }
        else
        {
            return SolveTopMiddle_CaseBottom(cubie);
        }
    }

    bool SolveTopMiddle_CaseTop(CubeInfo.Cubie cubie)
    {
        return _solved.Count < 4;
    }

    bool SolveTopMiddle_CaseMiddle(CubeInfo.Cubie cubie)
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
        ArrayList path = _solver.Search(_solved, steps);

        AnimatePath(path); // for visuals, changes display cube over many frames
        // apply state changes and get ready for next iteration
        ExecutePath(path); // changes planning cube immediately
        
        // error checking
        if (path.Count == 0)
        {
            Debug.LogError("Empty path returned");
        }
        return _solved.Count < 4;
    }

    bool SolveTopMiddle_CaseBottom(CubeInfo.Cubie cubie)
    {
        return _solved.Count < 4;
    }

    void AnimatePath(ArrayList path)
    {
        string s = "";
        for (int i = 0; i < path.Count; i++)
        {
            if ((string) path[i] == "") continue;
            s += path[i] + " ";
        }
        Debug.Log("FOUND " + s);

        CubeDemo demo = gameObject.GetComponent<CubeDemo>();
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
}

using UnityEngine;
using System.Collections;

public class CubePlanner : MonoBehaviour {

    public GameObject scrap = null; // root of shadow cube who'se state mirrors the real cube; used for planning
    private ArrayList _shadowCube = new ArrayList();
    private CubeStateManager _cube = new CubeStateManager();
    private CubeInfo _cubies = new CubeInfo();
    private CubeTaskSolver _solver = new CubeTaskSolver();

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
    }


    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateCubeState();
            Solve();
        }
	}

    void Solve()
    {
        SolveTopMiddle();
        SolveTopCorners();
        SolveMiddleMiddle();
    }

    bool TopMiddleSolved(ref CubeInfo.Cubie c, ArrayList solvedCubes)
    {
        _cubies.AnalyzeTopMiddle(ref c, solvedCubes);
        return c == null;
    }

    void SolveTopMiddle()
    {
        int count = 0;
        CubeInfo.Cubie cubie = null;
        ArrayList solved = new ArrayList();
        while (!TopMiddleSolved(ref cubie, solved))
        {
            Debug.Log("FIX " + cubie.id + "  out of place");
            foreach (CubeInfo.Cubie c in solved)
            {
                Debug.Log(" But cubie " + c.id + " is ok!");
            }

            ArrayList steps = new ArrayList();
            string[] level1 = {"L", "L'", 
                               "R", "R'", 
                               "F", "F'", 
                               "B", "B'", "" };
            string[] level0 = { "U", "U'", "U2", "" };
            steps.Add(level0);
            steps.Add(level1);
            steps.Add(level0);
            steps.Add(level1);
            //steps.Add(level0);

            ArrayList constraints = solved;
            constraints.Add(cubie);
            ArrayList path = _solver.Search(constraints, steps);

            AnimatePath(path); // for visuals, changes display cube over many frames

            // apply state changes and get ready for next iteration
            ExecutePath(path); // changes planning cube immediately
            cubie = null;
            solved.Clear();
        
            // error checking
            if (path.Count == 0)
            {
                Debug.LogError("Empty path returned");
                break; // an error occured!
            }

            count++;
            if (count > 2)
            {
                Debug.LogError("SEARCH steps exceeded");
                break; // failsafe: error in logic!!
            }
        }
    }

    void SolveTopCorners()
    {
    }

    void SolveMiddleMiddle()
    {
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

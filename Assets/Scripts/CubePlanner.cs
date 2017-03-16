using UnityEngine;
using System.Collections;

public class CubePlanner : MonoBehaviour {

    public GameObject scrap = null; // hack for creating space transforms
    private ArrayList _scrap = new ArrayList();
    private CubeStateManager _cube = new CubeStateManager();
    private CubeInfo _cubies = new CubeInfo();
    private CubeTaskSolver _solver = new TopMiddleSolver();

    void Start()
    {
        CubeStateManager tmp = new CubeStateManager();
        tmp.init(transform);

        _cubies.init(gameObject.transform); // cube should be initialized before this call
        for (int i = 0; i < _cubies.GetNumCubes(); i++)
        {
            GameObject obj = GameObject.Instantiate(scrap);
            CubeInfo.Cubie info = _cubies.GetCubeInfo(i);
            obj.transform.localPosition = info.transform.localPosition;
            obj.transform.localRotation = info.transform.localRotation;
            _scrap.Add(obj.transform);
        }

        foreach (Transform t in _scrap)
        {
            t.parent = scrap.transform;
        }

        _cube.init(scrap.transform);
        _solver.init(_cubies, _cube);
    }

    void CopyCubeState()
    {
        for (int i = 0; i < _scrap.Count; i++)
        {
            Transform t = _scrap[i] as Transform;
            CubeInfo.Cubie info = _cubies.GetCubeInfo(i);
            t.localPosition = info.transform.localPosition;
            t.localRotation = info.transform.localRotation;
        }
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CopyCubeState();
            ArrayList path = _solver.Search(_scrap);
            AnimatePath(path);
        }
	}


    void AnimatePath(ArrayList path)
    {
        string s = "";
        for (int i = 0; i < path.Count; i++)
        {
            s += path[i] + " ";
        }
        Debug.Log("FOUND " + s);

        CubeDemo demo = gameObject.GetComponent<CubeDemo>();
        demo.ParseRecipe(s);
    }

}

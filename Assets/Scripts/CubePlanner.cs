using UnityEngine;
using System.Collections;

public class CubePlanner : MonoBehaviour {

    public GameObject scrap = null; // root of shadow cube who'se state mirrors the real cube; used for planning
    private ArrayList _shadowCube = new ArrayList();
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
            _shadowCube.Add(obj.transform);
        }

        foreach (Transform t in _shadowCube)
        {
            t.parent = scrap.transform;
        }

        _cube.init(scrap.transform);
        _solver.init(_cubies, _cube, _shadowCube);
    }


    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ArrayList path = _solver.Search();
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

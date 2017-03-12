using UnityEngine;
using System.Collections;

public class CubePlanner : MonoBehaviour {

    public GameObject scrap = null; // hack for creating space transforms
    private ArrayList _scrap = new ArrayList();
    private ArrayList _idList = new ArrayList();

    private string[] _moves = {"L", "L'", "L2",
                               "R", "R'", "R2",
                               "F", "F'", "F2",
                               "B", "B'", "B2",
                               "U", "U'", "U2",
                               "D", "D'", "D2" };

    private ArrayList posX = new ArrayList();
    private ArrayList negX = new ArrayList();
    private ArrayList zeroX = new ArrayList();

    private Vector3 cPosX = new Vector3();
    private Vector3 cNegX = new Vector3();
    private Vector3 cZeroX = new Vector3();

    private ArrayList posY = new ArrayList();
    private ArrayList negY = new ArrayList();
    private ArrayList zeroY = new ArrayList();

    private Vector3 cPosY = new Vector3();
    private Vector3 cNegY = new Vector3();
    private Vector3 cZeroY = new Vector3();

    private ArrayList posZ = new ArrayList();
    private ArrayList negZ = new ArrayList();
    private ArrayList zeroZ = new ArrayList();

    private Vector3 cPosZ = new Vector3();
    private Vector3 cNegZ = new Vector3();
    private Vector3 cZeroZ = new Vector3();

    class SearchNode
    {
        private ArrayList _ids;
        private ArrayList _pos = new ArrayList();
        private ArrayList _rot = new ArrayList();
        private string _move = "";
        private SearchNode _parent = null;
        private int _score = 0;

        public SearchNode(ArrayList ids)
        {
            _ids = ids;
            for (int i = 0; i < ids.Count; i++)
            {
                _pos.Add(new Vector3());
                _rot.Add(new Vector3());
            }
        }

        public void LoadState(ArrayList objects)
        {
            for (int i = 0; i < _ids.Count; i++)
            {
                int id = (int) _ids[i];
                (objects[id] as Transform).localPosition = (Vector3) _pos[i];
                (objects[id] as Transform).localRotation = Quaternion.Euler((Vector3) _rot[i]);
            }
        }

        public void SaveState(ArrayList objects)
        {
            for (int i = 0; i < _ids.Count; i++)
            {
                int id = (int) _ids[i];
                _pos[i] = (objects[id] as Transform).localPosition;
                _rot[i] = (objects[id] as Transform).localRotation.eulerAngles;
            }
        }

        public void SetCubeState(int id, Vector3 desiredPosition, Vector3 desiredRotation)
        {
            for (int i = 0; i < _ids.Count; i++)
            {
                if ((int) _ids[i] == id)
                {
                    _pos[i] = desiredPosition;
                    _rot[i] = desiredRotation;
                }
            }
        }

        public bool IsEqual(SearchNode other)
        {
            for (int i = 0; i < _ids.Count; i++)
            {
                if (((Vector3) _pos[i] - (Vector3) other._pos[i]).magnitude > 0.001f) return false;
                if (((Vector3) _rot[i] - (Vector3) other._rot[i]).magnitude > 0.001f) return false;
            }
            return true;
        }

        public override string ToString()
        {
            string s = "--------\n";
            for (int i = 0; i < _ids.Count; i++)
            {
                s += _ids[i] + " " + (Vector3)_pos[i] + " " + (Vector3)_rot[i] + "\n";
            }
            return s;
        }

        public SearchNode parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public string move
        {
            get { return _move; }
            set { _move = value; }
        }

        public int score
        {
            get { return _score; }
            set { _score = value; }
        }
    };

    class TaskSolver
    {
        CubeController _controller = null;
        public TaskSolver(CubeController controller)
        {
            _controller = controller;
        }

        public bool Criteria()
        {
            return false;
        }
 
    };

    void Start()
    {
	}
	
    void CacheCubeState(CubeController controller)
    {
        if (_scrap.Count == 0)
        {
            for (int i = 0; i < controller.GetNumCubes(); i++)
            {
                GameObject obj = GameObject.Instantiate(scrap);
                CubeController.CubeInfo info = controller.GetCubeInfo(i);
                obj.transform.localPosition = info.transform.localPosition;
                obj.transform.localRotation = info.transform.localRotation;
                _scrap.Add(obj.transform);
            }

            SortCubeGroups();

            cZeroX = ComputeCenter(zeroX);
            cPosX = ComputeCenter(posX);
            cNegX = ComputeCenter(negX);

            cZeroY = ComputeCenter(zeroY);
            cPosY = ComputeCenter(posY);
            cNegY = ComputeCenter(negY);

            cZeroZ = ComputeCenter(zeroZ);
            cPosZ = ComputeCenter(posZ);
            cNegZ = ComputeCenter(negZ);
        }

        for (int i = 0; i < controller.GetNumCubes(); i++)
        {
            Transform obj = _scrap[i] as Transform;
            obj.localPosition = controller.GetCubeInfo(i).transform.localPosition;
            obj.localRotation = controller.GetCubeInfo(i).transform.localRotation;
        }
    }

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CubeController controller = gameObject.GetComponent<CubeController>();
            CacheCubeState(controller);
            Search(_scrap);

            /*
            ArrayList list;
            Vector3 center;
            Vector3 axis;
            float amount;
            TranslateMove("D'", out list, out center, out axis, out amount);
            turn(list, center, axis, amount); // un-apply move
            turn(list, center, axis, -amount); // un-apply move
            */
        }
	}

    bool IsMiddleCube(CubeController.CubeInfo info)
    {
        int zeroCount = 0;
        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(info.homePos[i]) < 0.001f) zeroCount++;
        }

        return zeroCount == 2;
    }

    void TranslateMove(string word, out ArrayList list, out Vector3 center, out Vector3 axis, out float amount)
    {
        list = new ArrayList();
        center = new Vector3();
        axis = new Vector3();
        amount = 0.0f;

        if (word == "F") { list = posX; center = cPosX; axis = new Vector3(1, 0, 0); amount = 90; }
        else if (word == "F'") { list = posX; center = cPosX; axis = new Vector3(-1, 0, 0); amount = 90; }
        else if (word == "F2") { list = posX; center = cPosX; axis = new Vector3(1, 0, 0); amount = 180; }
        else if (word == "B") { list = negX; center = cNegX; axis = new Vector3(-1, 0, 0); amount = 90; }
        else if (word == "B'") { list = negX; center = cNegX; axis = new Vector3(1, 0, 0); amount = 90; }
        else if (word == "B2") { list = negX; center = cNegX; axis = new Vector3(-1, 0, 0); amount = 180; }
        else if (word == "U") { list = posY; center = cPosY; axis = new Vector3(0, 1, 0); amount = 90; }
        else if (word == "U'") { list = posY; center = cPosY; axis = new Vector3(0, -1, 0); amount = 90; }
        else if (word == "U2") { list = posY; center = cPosY; axis = new Vector3(0, 1, 0); amount = 180; }
        else if (word == "D") { list = negY; center = cNegY; axis = new Vector3(0, -1, 0); amount = 90; }
        else if (word == "D'") { list = negY; center = cNegY; axis = new Vector3(0, 1, 0); amount = 90; }
        else if (word == "D2") { list = negY; center = cNegY; axis = new Vector3(0, -1, 0); amount = 180; }
        else if (word == "R") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, 1); amount = 90; }
        else if (word == "R'") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, -1); amount = 90; }
        else if (word == "R2") { list = posZ; center = cPosZ; axis = new Vector3(0, 0, 1); amount = 180; }
        else if (word == "L") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, -1); amount = 90; }
        else if (word == "L'") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, 1); amount = 90; }
        else if (word == "L2") { list = negZ; center = cNegZ; axis = new Vector3(0, 0, -1); amount = 180; }
        else Debug.Log("Unknown command passed to ApplyMove!");
    }

    void ComputePath(ArrayList path)
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

    bool Contains(ArrayList list, SearchNode node)
    {
        for (int i = 0; i < list.Count; i++)
        {
             SearchNode visitedNode = list[i] as SearchNode;
             if (visitedNode.IsEqual(node)) return true;
        }
        return false;
    }

    void SortCubeGroups()
    {
        zeroX.Clear();
        zeroY.Clear();
        zeroZ.Clear();

        posX.Clear();
        posY.Clear();
        posZ.Clear();

        negX.Clear();
        negY.Clear();
        negZ.Clear();

        foreach (Transform child in _scrap)
        {
             Check(child, 0, 0.0f, zeroX);
             Check(child, 0, 1.25f, posX);
             Check(child, 0, -1.25f, negX);

             Check(child, 1, 0.0f, zeroY);
             Check(child, 1, 1.25f, posY);
             Check(child, 1, -1.25f, negY);

             Check(child, 2, 0.0f, zeroZ);
             Check(child, 2, 1.25f, posZ);
             Check(child, 2, -1.25f, negZ);
        }
    }

    void Check(Transform t, int idx, float desired, ArrayList list)
    {
        Vector3 pos = t.localPosition;
        if (Mathf.Abs(pos[idx] - desired) < 0.25f)
        {
            // Adjust position so it's "perfect"
            pos[idx] = desired;
            t.localPosition = pos;
            list.Add(t);
        }
    }

    Vector3 ComputeCenter(ArrayList transforms)
    {
        Vector3 ave = new Vector3(0, 0, 0);
        foreach (Transform t in transforms)
        {
            ave += t.localPosition;
        }
        ave /= transforms.Count;
        //Debug.Log(ave);
        return ave;
   }

   void turn(ArrayList list, Vector3 center, Vector3 axis, float amount)
   {
        foreach (Transform t in list)
        {
            Quaternion q = Quaternion.AngleAxis(amount, axis) * t.localRotation;
            t.RotateAround(center, axis, amount);
            t.localRotation = q;
        }
        SortCubeGroups();
   }

    public ArrayList Search(ArrayList world)
    {
        ArrayList steps = new ArrayList();
        /*
        string[] level1 = {"L", "L'", "L2",
                               "R", "R'", "R2",
                               "F", "F'", "F2",
                               "B", "B'", "B2" };
        string[] level2 = { "U", "U'", "U2" };
        steps.Add(level1);
        steps.Add(level2);
        steps.Add(level1);
        steps.Add(level2);*/
        string[] level1 = {"L", "L'", "L2",
                               "R", "R'", "R2",
                               "F", "F'", "F2",
                               "B", "B'", "B2" };
        steps.Add(level1);

        CubeController controller = gameObject.GetComponent<CubeController>();
        CubeController.CubeInfo info = controller.FindCube(new Vector3(1.25f, 1.25f, 0));
        ArrayList constraints = new ArrayList();
        constraints.Add(info.id);

        ArrayList path = new ArrayList();
        int s = Search(world, constraints, steps, 0, ref path);
        Debug.Log("Found path: " + s);
        ComputePath(path);
        return path;
    }

    public int Search(ArrayList worldState, ArrayList constraints, ArrayList steps, int stepNum, ref ArrayList path)
    {
        int score = ScoreState(worldState, constraints);
        ArrayList best = new ArrayList(path);

        if (stepNum >= steps.Count)
        {
            Debug.Log("  score " + score+" "+path[0]);
            return score;
        }

        string[] turns = (string[])steps[stepNum];
        for (int i = 0; i < turns.Length; i++)
        {
            ArrayList list;
            Vector3 center;
            Vector3 axis;
            float amount;

            TranslateMove(turns[i], out list, out center, out axis, out amount);
            path.Add(turns[i]);

            turn(list, center, axis, amount); // un-apply move
            int tmp = Search(worldState, constraints, steps, stepNum + 1, ref path);
            if (tmp > score)
            {
                score = tmp;
                best = new ArrayList(path);
            }
            turn(list, center, axis, -amount); // un-apply move
            path.RemoveAt(path.Count - 1);
        }
        path = new ArrayList(best);
        return score;
    }

    public int ScoreState(ArrayList worldState, ArrayList constraints)
    {
        CubeController controller = gameObject.GetComponent<CubeController > ();

        bool requirements = true;
        for (int i = 0; i < constraints.Count && requirements; i++)
        {
            CubeController.CubeInfo info = controller.GetCubeInfo((int) constraints[i]);
            Transform t = worldState[info.id] as Transform;
            if ((info.homePos - t.localPosition).magnitude > 0.001f) requirements = false;
            if ((info.homeRot - t.localRotation.eulerAngles).magnitude > 0.001f) requirements = false; 
        }


        if (!requirements) return 0;

        int score = 0;
        for (int i = 0; i < controller.GetNumCubes(); i++)
        {
            CubeController.CubeInfo info = controller.GetCubeInfo(i);
            if ((info.homePos - (worldState[i] as Transform).localPosition).magnitude < 0.001f) score++;
            if ((info.homeRot - (worldState[i] as Transform).localRotation.eulerAngles).magnitude < 0.001f) score++;
        }

        return score;
    }
}

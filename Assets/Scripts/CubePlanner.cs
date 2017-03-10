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
    };

    void Start() {

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

            CubeController.CubeInfo info = controller.FindCube(new Vector3(1.25f, 1.25f, -1.25f)); // front, top, left

            _idList = new ArrayList();
            _idList.Add(info.id);

            SearchNode end = new SearchNode(_idList); // move one cube, one move
            end.SetCubeState(info.id, info.homePos, info.homeRot);

            SearchNode start = new SearchNode(_idList);
            start.SaveState(_scrap);

            Debug.Log("START " + start);
            Debug.Log("END " + end);

            Search(start, end);
        }
	}

    SearchNode ApplyMove(SearchNode node, string word)
    {
        ArrayList list = new ArrayList();
        Vector3 center = new Vector3();
        Vector3 axis = new Vector3();
        float amount = 0.0f;

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

        turn(list, center, axis, amount); // apply move
        SearchNode nextState = new SearchNode(_idList); // cache state change
        nextState.SaveState(_scrap);
        nextState.parent = node;
        nextState.move = word;
        turn(list, center, axis, -amount); // un-apply move
        return nextState;
   }

   void Search(SearchNode start, SearchNode end)
   {
        if (start.IsEqual(end))
        {
            Debug.Log("Search: No work to do!");
            return;
        }

        Queue queue = new Queue();
        queue.Enqueue(start);

        ArrayList visited = new ArrayList();
        bool found = false;
        for (int depth = 0; depth < 3 && !found; depth++)
        {
            SearchNode current = queue.Dequeue() as SearchNode;
            visited.Add(current);

            current.LoadState(_scrap);

            foreach (string move in _moves)
            {
                SearchNode next = ApplyMove(current, move);
                if (next.IsEqual(end))
                {
                    ComputePath(next);
                    found = true;
                    break;
                }
                Debug.Log("NEXT " + " "+next.move+" "+next);

                if (!Contains(visited, next)) queue.Enqueue(next);
            }
        }

        Debug.Log("Search done");
    }

    void ComputePath(SearchNode end)
    {
        SearchNode node = end;
        string s = node.move;
        while (node.parent != null)
        {
            node = node.parent;
            s = node.move + " " + s;
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
            t.RotateAround(center, axis, amount);
        }
        SortCubeGroups();
   }
}

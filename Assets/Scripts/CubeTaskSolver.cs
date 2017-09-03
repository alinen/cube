using UnityEngine;
using System.Collections;

public class CubeTaskSolver
{
    private CubeStateManager _cube;
    private CubeInfo _cubies;
    private ArrayList _shadowCube;

    public CubeTaskSolver() { }

    public void init(CubeInfo cubies, CubeStateManager scrap, ArrayList shadow)
    {
        _cube = scrap;
        _cubies = cubies;
        _shadowCube = shadow;
    }

    public ArrayList Search(ArrayList constraints, ArrayList steps)
    {
        ArrayList path = new ArrayList();
        ArrayList best = new ArrayList();
        int s = Search(constraints, steps, 0, ref path, ref best);
        //Debug.Log("Found path with score " + s);
        return best;
    }

    private int ScoreState(ArrayList constraints)
    {
        bool requirements = true;
        for (int i = 0; i < constraints.Count && requirements; i++)
        {
            CubeInfo.Cubie info = constraints[i] as CubeInfo.Cubie;
            Transform t = _shadowCube[info.id] as Transform;
            if ((info.homePos - t.localPosition).magnitude > 0.001f) requirements = false;
            if ((info.homeRot - t.localRotation.eulerAngles).magnitude > 0.001f) requirements = false;
        }

        if (!requirements) return 0;

        int score = 0;
        for (int i = 0; i < _cubies.GetNumCubes(); i++)
        {
            CubeInfo.Cubie info = _cubies.GetCubeInfo(i);
            if ((info.homePos - (_shadowCube[i] as Transform).localPosition).magnitude < 0.001f) score++;
            if ((info.homeRot - (_shadowCube[i] as Transform).localRotation.eulerAngles).magnitude < 0.001f) score++;
        }

        return score;
    }

    private int Search(ArrayList constraints, ArrayList steps, int stepNum, ref ArrayList path, ref ArrayList bestPath)
    {
        int score = ScoreState(constraints);
        if (stepNum >= steps.Count)
        {
            //Debug.Log("  score " + score + " " + path[0]);
            if (score > 0)
            {
                // bonus for empty moves (e.g. fewer moves is better)
                foreach (string s in path)
                {
                    if (s == "") score += 10;
                }
                //string spath = "";
                //foreach (string s in path) spath += s + " ";
                //Debug.Log("SCORE " + score + " " + spath + " " + path.Count);
            }
            bestPath = new ArrayList(path);
            return score;
        }

        string[] turns = (string[])steps[stepNum];
        for (int i = 0; i < turns.Length; i++)
        {
            ArrayList list = new ArrayList();
            Vector3 center = new Vector3(0,0,0);
            Vector3 axis = new Vector3(0,0,0);
            float amount = 0.0f;

            string turn = turns[i];
            if (turn != "")
            {
                if (turn == "X") turn = _cube.Reverse((string) path[path.Count - 2]);
                _cube.CmdToTurn(turn, out list, out center, out axis, out amount);
                _cube.turn(list, center, axis, amount); // un-apply move
            }
            path.Add(turn);
            ArrayList bestChild = new ArrayList();
            int tmp = Search(constraints, steps, stepNum + 1, ref path, ref bestChild);
            if (tmp > score)
            {
                score = tmp;
                bestPath = new ArrayList(bestChild);
            }
            if (turn != "") _cube.turn(list, center, axis, -amount); // un-apply move
            path.RemoveAt(path.Count - 1);
        }
        return score;
    }
};


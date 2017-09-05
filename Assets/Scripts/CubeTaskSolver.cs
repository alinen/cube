using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeTaskSolver
{
    public delegate int ScoreStateFn(CubeInfo.Cubie cubie, List<CubeInfo.Cubie> constraints);
    private CubeStateManager _cube;
    private CubeInfo _cubies;

    public CubeTaskSolver() { }

    public void init(CubeInfo cubies, CubeStateManager scrap)
    {
        _cube = scrap;
        _cubies = cubies;
    }

    public List<string> Search(CubeInfo.Cubie c, 
        List<CubeInfo.Cubie> constraints, ArrayList steps, ScoreStateFn scoreFn)
    {
        List<string> path = new List<string>();
        List<string> best = new List<string>();
        int s = Search(c, constraints, steps, 0, scoreFn, ref path, ref best);
        return best;
    }

    public int ScoreCubieSolved(CubeInfo.Cubie cubie, List<CubeInfo.Cubie> constraints)
    {
        bool requirements = _cubies.IsSolved(cubie);
        for (int i = 0; i < constraints.Count && requirements; i++)
        {
            CubeInfo.Cubie info = constraints[i] as CubeInfo.Cubie;
            if (!_cubies.IsSolved(info)) requirements = false;
        }

        if (!requirements) return 0;

        int score = 0;
        for (int i = 0; i < _cubies.GetNumCubes(); i++)
        {
            CubeInfo.Cubie info = _cubies.GetCubeInfo(i);
            if (_cubies.CorrectPos(info)) score++;
            if (_cubies.CorrectOri(info)) score++; 
        }
        return score;
    }

    public int ScorePositions(CubeInfo.Cubie cubie, List<CubeInfo.Cubie> constraints)
    {
        bool requirements = cubie != null? _cubies.CorrectPos(cubie) : true;
        for (int i = 0; i < constraints.Count && requirements; i++)
        {
            CubeInfo.Cubie info = constraints[i] as CubeInfo.Cubie;
            if (!_cubies.IsSolved(info)) requirements = false;
        }

        if (!requirements) return 0;

        int score = 0;
        for (int i = 0; i < _cubies.GetNumCubes(); i++)
        {
            CubeInfo.Cubie info = _cubies.GetCubeInfo(i);
            if (_cubies.CorrectPos(info)) score++;
        }
        return score;
    }

    // Solve for cubie given the passed constraints
    private int Search(CubeInfo.Cubie c, List<CubeInfo.Cubie> constraints, 
        ArrayList steps, int stepNum, ScoreStateFn scoreFn, 
        ref List<string> path, ref List<string> bestPath)
    {
        int score = scoreFn(c, constraints);
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
            bestPath = new List<string>(path);
            return score;
        }

        string[] turns = (string[])steps[stepNum];
        for (int i = 0; i < turns.Length; i++)
        {
            List<Transform> list = new List<Transform>();
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
            List<string> bestChild = new List<string>();
            int tmp = Search(c, constraints, steps, stepNum + 1, scoreFn, ref path, ref bestChild);
            if (tmp > score)
            {
                score = tmp;
                bestPath = new List<string>(bestChild);
            }
            if (turn != "") _cube.turn(list, center, axis, -amount); // un-apply move
            path.RemoveAt(path.Count - 1);
        }
        return score;
    }
};


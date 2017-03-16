﻿using UnityEngine;
using System.Collections;

public abstract class CubeTaskSolver
{
    protected CubeStateManager _cube;
    protected CubeInfo _cubies;

    public CubeTaskSolver() { }
    public abstract ArrayList Search(ArrayList world);

    public void init(CubeInfo cubies, CubeStateManager scrap)
    {
        _cube = scrap;
        _cubies = cubies;
    }

    protected int ScoreState(ArrayList worldState, ArrayList constraints)
    {
        bool requirements = true;
        for (int i = 0; i < constraints.Count && requirements; i++)
        {
            CubeInfo.Cubie info = _cubies.GetCubeInfo((int)constraints[i]);
            Transform t = worldState[info.id] as Transform;
            if ((info.homePos - t.localPosition).magnitude > 0.001f) requirements = false;
            if ((info.homeRot - t.localRotation.eulerAngles).magnitude > 0.001f) requirements = false;
        }

        if (!requirements) return 0;

        int score = 0;
        for (int i = 0; i < _cubies.GetNumCubes(); i++)
        {
            CubeInfo.Cubie info = _cubies.GetCubeInfo(i);
            if ((info.homePos - (worldState[i] as Transform).localPosition).magnitude < 0.001f) score++;
            if ((info.homeRot - (worldState[i] as Transform).localRotation.eulerAngles).magnitude < 0.001f) score++;
        }

        return score;
    }

    protected int Search(ArrayList worldState, ArrayList constraints, ArrayList steps, int stepNum, ref ArrayList path)
    {
        int score = ScoreState(worldState, constraints);
        ArrayList best = new ArrayList(path);

        if (stepNum >= steps.Count)
        {
            Debug.Log("  score " + score + " " + path[0]);
            return score;
        }

        string[] turns = (string[])steps[stepNum];
        for (int i = 0; i < turns.Length; i++)
        {
            ArrayList list;
            Vector3 center;
            Vector3 axis;
            float amount;

            _cube.CmdToTurn(turns[i], out list, out center, out axis, out amount);
            path.Add(turns[i]);

            _cube.turn(list, center, axis, amount); // un-apply move
            int tmp = Search(worldState, constraints, steps, stepNum + 1, ref path);
            if (tmp > score)
            {
                score = tmp;
                best = new ArrayList(path);
            }
            _cube.turn(list, center, axis, -amount); // un-apply move
            path.RemoveAt(path.Count - 1);
        }
        path = new ArrayList(best);
        return score;
    }

};

public class TopMiddleSolver : CubeTaskSolver 
{
    public TopMiddleSolver() : base()
    {
    }

    public override ArrayList Search(ArrayList world)
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
        string[] level1 = { "L", "L'", "L2",
                            "R", "R'", "R2",
                            "F", "F'", "F2",
                            "B", "B'", "B2" };
        steps.Add(level1);

        CubeInfo.Cubie info = _cubies.FindCube(new Vector3(1.25f, 1.25f, 0));
        ArrayList constraints = new ArrayList();
        constraints.Add(info.id);

        ArrayList path = new ArrayList();
        int s = Search(world, constraints, steps, 0, ref path);
        Debug.Log("Found path with score " + s);
        return path;
    }
}

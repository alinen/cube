using UnityEngine;
using System.Collections.Generic;

public class CubeTask
{
    public delegate bool SolveFn(ref List<string> path);

    private SolveFn _solvefn;
    public CubeTask(SolveFn sfn)
    {
        _solvefn = sfn;
    }

    public bool Solve(ref List<string> path)
    {
        return _solvefn(ref path);
    }
}

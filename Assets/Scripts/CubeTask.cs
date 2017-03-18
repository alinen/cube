using UnityEngine;
using System.Collections;

public class CubeTask
{
    public delegate bool SolveFn(ref ArrayList path);

    private SolveFn _solvefn;
    public CubeTask(SolveFn sfn)
    {
        _solvefn = sfn;
    }

    public bool Solve(ref ArrayList path)
    {
        return _solvefn(ref path);
    }
}

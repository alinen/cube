using UnityEngine;
using System.Collections;

public class CubeTask
{
    public delegate bool SolveFn();

    private SolveFn _solvefn;
    public CubeTask(SolveFn sfn)
    {
        _solvefn = sfn;
    }

    public bool Solve()
    {
        return _solvefn();
    }
}

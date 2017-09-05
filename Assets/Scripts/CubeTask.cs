using UnityEngine;
using System.Collections.Generic;

public class CubeTask
{
    public delegate bool SolveFn(ref List<string> path);
    public delegate bool SuccessCriteriaFn();

    private SolveFn _solvefn;
    private SuccessCriteriaFn _criteriafn;

    public CubeTask(SolveFn sfn, SuccessCriteriaFn scfn)
    {
        _solvefn = sfn;
        _criteriafn = scfn;
    }

    public bool Finished()
    {
        return _criteriafn();
    }

    public bool Solve(ref List<string> path)
    {
        return _solvefn(ref path);
    }
}

using UnityEngine;
using System.Collections.Generic;

public class CubeTask
{
    public delegate bool SolveFn(ref List<string> path);
    public delegate bool SuccessCriteriaFn();
    public string rowName = "";
    public string taskName = "";

    private SolveFn _solvefn;
    private SuccessCriteriaFn _criteriafn;

    public CubeTask(SolveFn sfn, SuccessCriteriaFn scfn, string rName, string tName)
    {
        _solvefn = sfn;
        _criteriafn = scfn;
        rowName = rName;
        taskName = tName;
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

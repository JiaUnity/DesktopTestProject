using UnityEngine;
using UnityEngine.Diagnostics;


public class Crashy : MonoBehaviour
{
    public void AccessViolation()
    {
        Utils.ForceCrash(ForcedCrashCategory.AccessViolation);
    }

    public void AbortCrash()
    {
        Utils.ForceCrash(ForcedCrashCategory.Abort);
    }

    public void FatalError()
    {
        Utils.ForceCrash(ForcedCrashCategory.FatalError);
    }

    public void PureVirtual()
    {
        Utils.ForceCrash(ForcedCrashCategory.PureVirtualFunction);
    }
}

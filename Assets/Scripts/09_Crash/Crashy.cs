using UnityEngine;


public class Crashy : MonoBehaviour
{
    public void RaiseException()
    {
        Application.ForceCrash(0);
    }

    public void AbortCrash()
    {
        Application.ForceCrash(2);
    }

    public void PureVirtual()
    {
        Application.ForceCrash(3);
    }
}

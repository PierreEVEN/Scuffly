using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwitchBase : PlaneComponent
{
    public string Description = "";

    public abstract void Switch();

    public void StartOver()
    {
        gameObject.layer = 3;
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.layer = 3;
    }

    public void StopOver()
    {
        gameObject.layer = 0;
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.layer = 0;
    }
}

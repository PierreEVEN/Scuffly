using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarScreenManager : PlaneComponent
{
    public GameObject targetWidget;

    public List<RadarTargetManager> targets;

    private void OnEnable()
    {
        RadarComponent.OnDetectNewTarget.AddListener(OnDetectnewTarget);
        RadarComponent.OnLostTarget.AddListener(OnLostTarget);

        foreach (var target in RadarComponent.scannedTargets)
        {
            OnDetectnewTarget(target.Key);
        }

    }

    private void OnDisable()
    {
        RadarComponent.OnDetectNewTarget.RemoveListener(OnDetectnewTarget);
        RadarComponent.OnLostTarget.RemoveListener(OnDetectnewTarget);
    }

    void OnDetectnewTarget(GameObject target)
    {
        GameObject widget = Instantiate(targetWidget);
        RadarTargetManager targetScript = widget.GetComponent<RadarTargetManager>();
        targetScript.target = target;
        targets.Add(targetScript);
    }
    void OnLostTarget(GameObject target)
    {
        foreach(var item in targets)
        {
            if (item.target == target)
            {
                GameObject widget = item.gameObject;
                targets.Remove(item);
                Destroy(widget);
                return;
            }
        }
    }


    private void Update()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//@TODO improve threat warning indicator
[RequireComponent(typeof(Canvas))]
public class ThreatWarningIndicator : PlaneComponent
{
    Dictionary<GameObject, GameObject> displayedTargets = new Dictionary<GameObject, GameObject>();

    public GameObject RadarPointItem;

    Canvas container;
    private void OnEnable()
    {
        Plane.RadarComponent.OnDetectNewTarget.AddListener(AddTarget);
        Plane.RadarComponent.OnLostTarget.AddListener(RemoveTarget);

        container = GetComponent<Canvas>();
        foreach (var target in Plane.RadarComponent.scannedTargets)
            AddTarget(target.Key);
    }

    private void OnDisable()
    {
        Plane.RadarComponent.OnDetectNewTarget.RemoveListener(AddTarget);
        Plane.RadarComponent.OnLostTarget.RemoveListener(RemoveTarget);

        foreach (var target in displayedTargets)
            Destroy(target.Value);
        displayedTargets.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var target in Plane.RadarComponent.scannedTargets)
        {
            Vector3 directionToTarget = (target.Value.ScannedWorldPosition - Plane.transform.position);
            float angle = (Vector3.SignedAngle(new Vector3(directionToTarget.x, 0, directionToTarget.z), new Vector3(Plane.transform.forward.x, 0, Plane.transform.forward.z), new Vector3(0, 1, 0)) + 90) / 180 * Mathf.PI;
            float distance = (target.Value.ScannedWorldPosition - Plane.transform.position).magnitude;
            GameObject icon = displayedTargets[target.Key];
            icon.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized * Mathf.Min(40, distance / 400);
        }
    }

    void AddTarget(GameObject newTarget)
    {
        GameObject icon = Instantiate(RadarPointItem, container.transform);
        icon.transform.localScale = Vector3.one;
        displayedTargets.Add(newTarget, icon);
    }
    void RemoveTarget(GameObject removedTarget)
    {
        if (displayedTargets.ContainsKey(removedTarget))
        {
            Destroy(displayedTargets[removedTarget]);
            displayedTargets.Remove(removedTarget);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The threat warning indicator is a screen on the left of the front pannel of the cockpit that tell you who is around you
/// </summary>
[RequireComponent(typeof(Canvas))]
public class ThreatWarningIndicator : PlaneComponent
{
    /// <summary>
    /// A list of displayed target <TheTarget, TheWidget>
    /// </summary>
    Dictionary<GameObject, GameObject> displayedTargets = new Dictionary<GameObject, GameObject>();

    /// <summary>
    /// The UI instantied for each detected target
    /// </summary>
    public GameObject RadarPointItem;

    /// <summary>
    /// The object containing all the target widget
    /// </summary>
    Canvas container;

    private void OnEnable()
    {
        Plane.RadarComponent.OnDetectNewTarget.AddListener(AddTarget);
        Plane.RadarComponent.OnLostTarget.AddListener(RemoveTarget);

        // Add the target that was already scanned by the radar when this component whas spawned
        container = GetComponent<Canvas>();
        foreach (var target in Plane.RadarComponent.scannedTargets)
            AddTarget(target.Key);
    }

    private void OnDisable()
    {
        Plane.RadarComponent.OnDetectNewTarget.RemoveListener(AddTarget);
        Plane.RadarComponent.OnLostTarget.RemoveListener(RemoveTarget);

        // Destroy all the target widgets
        foreach (var target in displayedTargets)
            Destroy(target.Value);
        displayedTargets.Clear();
    }

    void Update()
    {
        // Update the position of each target's widget. The rotation is relative to the plane current heading direction
        foreach (var target in Plane.RadarComponent.scannedTargets)
        {
            Vector3 directionToTarget = (target.Value.ScannedWorldPosition - Plane.transform.position);
            float angle = (Vector3.SignedAngle(new Vector3(directionToTarget.x, 0, directionToTarget.z), new Vector3(Plane.transform.forward.x, 0, Plane.transform.forward.z), new Vector3(0, 1, 0)) + 90) / 180 * Mathf.PI;
            float distance = (target.Value.ScannedWorldPosition - Plane.transform.position).magnitude;
            GameObject icon = displayedTargets[target.Key];
            icon.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized * Mathf.Min(40, distance / 400);
        }
    }

    /// <summary>
    /// When the radar detect a new target
    /// </summary>
    /// <param name="newTarget"></param>
    void AddTarget(GameObject newTarget)
    {
        // Create a new enemy icon
        GameObject icon = Instantiate(RadarPointItem, container.transform);
        icon.transform.localScale = Vector3.one;
        displayedTargets.Add(newTarget, icon);
    }

    /// <summary>
    /// When the radar lost a target
    /// </summary>
    /// <param name="removedTarget"></param>
    void RemoveTarget(GameObject removedTarget)
    {
        if (displayedTargets.ContainsKey(removedTarget))
        {
            Destroy(displayedTargets[removedTarget]);
            displayedTargets.Remove(removedTarget);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The radar of the plane. It scan the targets around the plane
/// </summary>
public class Radar : PlaneComponent
{
    /// <summary>
    /// Target informations are refreshed and stored regularly in this structure
    /// </summary>
    public struct TargetMetaData
    {
        public Vector3 ScannedWorldPosition;
        public float scanTime;
    }

    /// <summary>
    /// A list of detected targets
    /// </summary>
    [HideInInspector]
    public Dictionary<GameObject, TargetMetaData> scannedTargets = new Dictionary<GameObject, TargetMetaData>();

    /// <summary>
    /// The current rotation of the radar / scan
    /// </summary>
    [HideInInspector]
    public float currentScanRotation = 0;

    /// <summary>
    /// Rotation speed of the radar
    /// </summary>
    public float scanSpeed = 200;

    /// <summary>
    /// If the target is not scanned again after "scanTimeout" seconds, it is considered as lost
    /// </summary>
    public float scanTimeout = 10;

    /// <summary>
    /// Event called when a target is detected or lost
    /// </summary>
    public UnityEvent<GameObject> OnDetectNewTarget = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> OnLostTarget = new UnityEvent<GameObject>();

    void Update()
    {
        UpdateTargets();
        float newRot = currentScanRotation + Time.deltaTime * scanSpeed;
        ScanVector(currentScanRotation % 360, newRot % 360);
        currentScanRotation = newRot;
    }

    /// <summary>
    /// Update a given target or add a new one
    /// </summary>
    /// <param name="target"></param>
    void UpdateTarget(GameObject target)
    {
        if (scannedTargets.ContainsKey(target))
        {
            var value = scannedTargets[target];
            value.scanTime = 0;
            value.ScannedWorldPosition = target.transform.position;
            scannedTargets[target] = value;
        }
        else
        {
            scannedTargets.Add(target, new TargetMetaData()
            {
                scanTime = 0,
                ScannedWorldPosition = target.transform.position
            });

            OnDetectNewTarget.Invoke(target);
        }
    }

    /// <summary>
    /// Override the y axis of a 3D vector
    /// </summary>
    /// <param name="inVector"></param>
    /// <param name="yAxis"></param>
    /// <returns></returns>
    static Vector3 OverrideYAxis(Vector3 inVector, float yAxis)
    {
        return new Vector3(inVector.x, yAxis, inVector.z);
    }

    /// <summary>
    /// Update all the targets that are in the current scan cone
    /// </summary>
    /// <param name="dirFrom"></param>
    /// <param name="dirTo"></param>
    void ScanVector(float dirFrom, float dirTo)
    {
        foreach (var plane in PlaneActor.PlaneList)
        {
            if (plane.gameObject == Plane.gameObject) continue;

            // Compute the cone
            plane.GetHeading();
            Vector3 relativeDirection = OverrideYAxis(plane.transform.position - Plane.transform.position, 0).normalized;
            float relativeAngle = (Vector3.SignedAngle(OverrideYAxis(relativeDirection, 0), OverrideYAxis(transform.forward, 0), new Vector3(0, 1, 0)) + 360) % 360;

            // Handle the modulo 360
            if (dirTo < dirFrom)
            {
                if (relativeAngle < dirTo)
                    relativeAngle += 360;
                dirTo += 360;
            }

            // Update the plane if it is in range
            if (dirFrom <= relativeAngle && relativeAngle < dirTo)
                UpdateTarget(plane.gameObject);
        }
    }

    void UpdateTargets()
    {
        // Handle the timeout for each targets

        var keys = new List<GameObject>(scannedTargets.Keys);
        foreach (var item in keys)
        {
            var value = scannedTargets[item];
            value.scanTime += Time.deltaTime;
            scannedTargets[item] = value;
        }

        List<GameObject> removedTargets = new List<GameObject>();
        foreach (var item in scannedTargets)
            if (item.Value.scanTime > scanTimeout)
                removedTargets.Add(item.Key);

        foreach (var item in removedTargets)
        {
            OnLostTarget.Invoke(item);
            scannedTargets.Remove(item);
        }
    }

}

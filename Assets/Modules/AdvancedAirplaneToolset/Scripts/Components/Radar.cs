using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Radar : PlaneComponent
{
    public struct TargetMetaData
    {
        public Vector3 ScannedWorldPosition;
        public float scanTime;
    }

    [HideInInspector]
    public Dictionary<GameObject, TargetMetaData> scannedTargets = new Dictionary<GameObject, TargetMetaData>();

    [HideInInspector]
    public float currentScanRotation = 0;

    public float scanSpeed = 200;
    public float scanTimeout = 10;

    public UnityEvent<GameObject> OnDetectNewTarget = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> OnLostTarget = new UnityEvent<GameObject>();

    // Update is called once per frame
    void Update()
    {
        UpdateTargets();
        float newRot = currentScanRotation + Time.deltaTime * scanSpeed;
        ScanVector(currentScanRotation % 360, newRot % 360);
        currentScanRotation = newRot;
    }

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
    public static Vector3 OverrideYAxis(Vector3 inVector, float yAxis)
    {
        return new Vector3(inVector.x, yAxis, inVector.z);
    }

    void ScanVector(float dirFrom, float dirTo)
    {
        foreach (var plane in PlaneActor.PlaneList)
        {
            if (plane.gameObject == Plane.gameObject) continue;

            plane.GetHeading();

            Vector3 relativeDirection = OverrideYAxis(plane.transform.position - Plane.transform.position, 0).normalized;

            float relativeAngle = (Vector3.SignedAngle(OverrideYAxis(relativeDirection, 0), OverrideYAxis(transform.forward, 0), new Vector3(0, 1, 0)) + 360) % 360;

            if (dirTo < dirFrom)
            {
                if (relativeAngle < dirTo)
                    relativeAngle += 360;
                dirTo += 360;
            }

            if (dirFrom <= relativeAngle && relativeAngle < dirTo)
            {
                UpdateTarget(plane.gameObject);
            }
        }

    }

    void UpdateTargets()
    {
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

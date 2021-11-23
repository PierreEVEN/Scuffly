using System.Collections.Generic;
using UnityEngine;

public class PlaneRadar : PlaneComponent
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

    public float scanSpeed = 50;
    public float scanTimeout = 10;

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
        }
    }
    public static Vector3 OverrideYAxis(Vector3 inVector, float yAxis)
    {
        return new Vector3(inVector.x, yAxis, inVector.z);
    }

    void ScanVector(float dirFrom, float dirTo)
    {
        foreach (var plane in PlaneManager.PlaneList)
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
        foreach (var item in scannedTargets)
        {
            var value = item.Value;
            value.scanTime += Time.deltaTime;
            scannedTargets[item.Key] = value;
        }

        List<GameObject> removedTargets = new List<GameObject>();
        foreach (var item in scannedTargets)
            if (item.Value.scanTime > scanTimeout)
                removedTargets.Add(item.Key);

        foreach (var item in removedTargets)
            scannedTargets.Remove(item);
    }
}

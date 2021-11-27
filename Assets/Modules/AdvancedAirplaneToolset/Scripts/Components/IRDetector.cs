using UnityEngine;

public class IRDetector : PlaneComponent
{

    public float DetectionAngle = 2.5f;
    public float LoseAngle = 6;
    public float DetectionRange = 10000;

    [HideInInspector]
    public GameObject acquiredTarget;

    void Update()
    {
        if (acquiredTarget)
        {
            if (!IsTargetInForwardCone(LoseAngle, DetectionRange, acquiredTarget))
            {
                acquiredTarget = null;
            }
        }
        else
        {
            foreach (var plane in PlaneActor.PlaneList)
            {
                if (plane == Plane)
                    continue;
                if (IsTargetInForwardCone(DetectionAngle, DetectionRange, plane.gameObject))
                {
                    acquiredTarget = plane.gameObject;
                    break;
                }
            }
        }
        if (WeaponSystem.CurrentSelectedWeaponType == PodItemType.Missile_IR)
        {
            if (WeaponSystem.isActiveAndEnabled)
                if (WeaponSystem.CurrentWeaponMode == WeaponMode.Pod_Air)
                    if (WeaponSystem.CurrentSelectedWeaponType == PodItemType.Missile_IR)
                    {
                        WeaponSystem.acquiredTarget = acquiredTarget;
                        return;
                    }
            WeaponSystem.acquiredTarget = null;
        }
    }

    bool IsTargetInForwardCone(float angle, float maxDistance, GameObject target)
    {
        Vector3 dirToTarget = target.transform.position - Plane.transform.position;
        return Vector3.Angle(dirToTarget, Plane.transform.forward) <= angle && maxDistance > Vector3.Distance(target.transform.position, Plane.transform.position);
    }
}

using UnityEngine;

/// <summary>
/// Infra-Red detector of the fighter jet (it is required to use aim-9 missiles)
/// </summary>
public class IRDetector : PlaneComponent
{
    /// <summary>
    /// The detector will check every plane that is in front of it, in a cone radius of max length "DetectionRange"
    /// </summary>
    public float DetectionAngle = 2.5f;
    public float LoseAngle = 6; // The target is lost only if it leave a cone of "LoseAngle" radius
    public float DetectionRange = 10000;

    /// <summary>
    /// The target currently tracced by the component
    /// </summary>
    [HideInInspector]
    public GameObject acquiredTarget;

    void Update()
    {
        if (acquiredTarget)
        {
            // Ensure the acquired target is not out of range
            if (!IsTargetInForwardCone(LoseAngle, DetectionRange, acquiredTarget))
                acquiredTarget = null;
        }
        else
        {
            // Else : search for a new target in range
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

        // Transmit the acquired target to the weapon system of the plane.
        //@TODO : make the target system of weapon more realistic to simplify the system
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

    /// <summary>
    /// Check if a target is in a forward cone of angle radius, and maxDistance length
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="maxDistance"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    bool IsTargetInForwardCone(float angle, float maxDistance, GameObject target)
    {
        Vector3 dirToTarget = target.transform.position - Plane.transform.position;
        return Vector3.Angle(dirToTarget, Plane.transform.forward) <= angle && maxDistance > Vector3.Distance(target.transform.position, Plane.transform.position);
    }
}

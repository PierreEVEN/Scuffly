using UnityEngine;

// Capteur infrarouge d'un avion de chasse (a placer dans la heirarchie de l'avion si necessaire)
public class IRDetector : PlaneComponent
{
    // Le capteur detecte toute source d'energie dans un rayon alpha en face de lui.
    public float DetectionAngle = 2.5f;
    // Angle maximal au dela duquel la cible est perdue
    public float LoseAngle = 6;
    // Distance de detection (m)
    public float DetectionRange = 10000;

    // Cible accrochee
    [HideInInspector]
    public GameObject acquiredTarget;

    void Update()
    {
        if (acquiredTarget)
        {
            // Si une cible est acquise, verifie qu'elle est toujours a portee du capteur
            if (!IsTargetInForwardCone(LoseAngle, DetectionRange, acquiredTarget))
            {
                acquiredTarget = null;
            }
        }
        else
        {
            // Sinon on cherche une nouvelle cible a portee
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
                        // Transmet la cible au systeme d'armement
                        WeaponSystem.acquiredTarget = acquiredTarget;
                        return;
                    }
            // Uniquement si on a selectionné le bon mode 
            WeaponSystem.acquiredTarget = null;
        }
    }

    // Verifie si une cible est dans un cone d'angle 'angle' en face du capteur
    bool IsTargetInForwardCone(float angle, float maxDistance, GameObject target)
    {
        Vector3 dirToTarget = target.transform.position - Plane.transform.position;
        return Vector3.Angle(dirToTarget, Plane.transform.forward) <= angle && maxDistance > Vector3.Distance(target.transform.position, Plane.transform.position);
    }
}

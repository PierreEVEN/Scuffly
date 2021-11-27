using UnityEngine;

/**
 * Classe de base pour tout component faisant partie d'un avion
 */
public class PlaneComponent : MonoBehaviour
{
    PlaneActor _plane;
    Rigidbody _physics;
    WeaponManager _weaponSystem;
    Radar _radar;
    IRDetector _irDetector;

    public Rigidbody Physics
    {
        get
        {
            if (!_physics)
            {
                _physics = GetComponentInParent<Rigidbody>();
            }
            return _physics;
        }
    }

    public PlaneActor Plane
    {
        get
        {
            if (!_plane)
            {
                _plane = GetComponentInParent<PlaneActor>();
            }
            return _plane;
        }
    }

    public WeaponManager WeaponSystem
    {
        get
        {
            if (!_weaponSystem)
            {
                _weaponSystem = GetComponentInParent<WeaponManager>();
            }
            return _weaponSystem;
        }
    }
    public Radar RadarComponent
    {
        get
        {
            if (!_radar)
            {
                _radar = Plane.GetComponentInChildren<Radar>();
            }
            return _radar;
        }
    }

    public IRDetector IrDetectorComponent
    {
        get
        {
            if (!_irDetector)
            {
                _irDetector = Plane.GetComponentInChildren<IRDetector>();
            }
            return _irDetector;
        }
    }
}

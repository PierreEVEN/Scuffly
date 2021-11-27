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
                if (!_physics && Application.isPlaying) Debug.LogError("failed to find RigidBody in parents");
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
                if (!_plane && Application.isPlaying) Debug.LogError("failed to find PlaneManager in parents");
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
                if (!_weaponSystem && Application.isPlaying) Debug.LogError("failed to find WeaponManager in parents");
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
                if (!_radar)
                    Debug.LogError("failed to find radar on plane");
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
                if (!_irDetector)
                    Debug.LogError("failed to find irDetector on plane");
            }
            return _irDetector;
        }
    }
}

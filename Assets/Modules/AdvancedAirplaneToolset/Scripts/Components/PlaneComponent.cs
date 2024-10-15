using UnityEngine;

/// <summary>
/// Base class of an airplane part. It contains some getters that can be used to get some informations about the owning plane.
/// </summary>
public class PlaneComponent : MonoBehaviour
{
    PlaneActor _plane;
    Rigidbody _physics;
    WeaponManager _weaponSystem;
    Radar _radar;
    IRDetector _irDetector;

    /// <summary>
    /// The rigidbody of the plane
    /// </summary>
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

    /// <summary>
    /// The main script of the plane
    /// </summary>
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

    /// <summary>
    /// Get the weapon system if available
    /// </summary>
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

    /// <summary>
    /// Get the radar if available
    /// </summary>
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

    /// <summary>
    /// Get the infrared radar if available
    /// </summary>
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

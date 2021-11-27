using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDComponent : MonoBehaviour
{
    HUDManager _hud;
    PlaneActor _plane;
    Rigidbody _physics;
    WeaponManager _weaponSytem;
    Radar _radar;
    IRDetector _irDetector;
    public HUDManager HUD
    {
        get
        {
            if (!_hud)
            {
                _hud = GetComponentInParent<HUDManager>();
                if (!_hud)
                    Debug.LogError("failed to find HUDManager in parent hierarchy");
            }
            return _hud;
        }
    }
    public PlaneActor Plane
    {
        get
        {
            if (!_plane)
            {
                _plane = HUD.Plane;
                if (!_plane)
                    Debug.LogError("failed to find owning plane for HUD widgets");
            }
            return _plane;
        }
    }
    public Rigidbody Physic
    {
        get
        {
            if (!_physics)
            {
                _physics = HUD.Plane.GetComponent<Rigidbody>();
                if (!_physics)
                    Debug.LogError("failed to find rigidBody for HUD widgets");
            }
            return _physics;
        }
    }
    public WeaponManager WeaponSystem
    {
        get
        {
            if (!_weaponSytem)
            {
                _weaponSytem = HUD.Plane.GetComponentInChildren<WeaponManager>();
                if (!_weaponSytem)
                    Debug.LogError("failed to find weaponManager for HUD widgets");
            }
            return _weaponSytem;
        }
    }
    public Radar RadarComponent
    {
        get
        {
            if (!_radar)
            {
                _radar = HUD.Plane.GetComponentInChildren<Radar>();
                if (!_radar)
                    Debug.LogError("failed to find radar for HUD widgets");
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
                _irDetector = HUD.Plane.GetComponentInChildren<IRDetector>();
                if (!_irDetector)
                    Debug.LogError("failed to find irDetector for HUD widgets");
            }
            return _irDetector;
        }
    }
}

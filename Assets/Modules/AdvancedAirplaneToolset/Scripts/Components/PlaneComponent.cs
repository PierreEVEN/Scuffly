using UnityEngine;

/**
 * Classe de base pour tout component faisant partie d'un avion
 */
public class PlaneComponent : MonoBehaviour
{
    PlaneActor plane;

    Rigidbody physics;

    WeaponManager weaponSystem;

    public Rigidbody Physics
    {
        get
        {
            if (!physics)
            {
                physics = GetComponentInParent<Rigidbody>();
                if (!physics && Application.isPlaying) Debug.LogError("failed to find RigidBody in parents");
            }
            return physics;
        }
    }

    public PlaneActor Plane
    {
        get
        {
            if (!plane)
            {
                plane = GetComponentInParent<PlaneActor>();
                if (!plane && Application.isPlaying) Debug.LogError("failed to find PlaneManager in parents");
            }
            return plane;
        }
    }

    public WeaponManager WeaponSystem
    {
        get
        {
            if (!weaponSystem)
            {
                weaponSystem = GetComponentInParent<WeaponManager>();
                if (!plane && Application.isPlaying) Debug.LogError("failed to find WeaponManager in parents");
            }
            return weaponSystem;
        }
    }
}

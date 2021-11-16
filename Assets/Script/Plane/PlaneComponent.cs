using UnityEngine;

/**
 * Classe de base pour tout component faisant partie d'un avion
 */
public class PlaneComponent : MonoBehaviour
{
    PlaneManager plane;

    Rigidbody physics;

    public Rigidbody Physics
    {
        get
        {
            if (!physics)
            {
                physics = GetComponentInParent<Rigidbody>();
                if (!physics) Debug.LogError("failed to find RigidBody in parents");
            }
            return physics;
        }
    }

    public PlaneManager Plane
    {
        get
        {
            if (!plane)
            {
                plane = GetComponentInParent<PlaneManager>();
                if (!plane) Debug.LogError("failed to find PlaneManager in parents");
            }
            return plane;
        }
    }
}

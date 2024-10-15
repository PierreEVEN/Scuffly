using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;


/// <summary>
/// A damageable component is a part of the plain that can be detached in case of damage
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class DamageableComponent : MonoBehaviour
{
    /// <summary>
    /// Current health of the part
    /// </summary>
    public float health = 100;

    /// <summary>
    /// Fire effects
    /// </summary>
    public VisualEffectAsset smokeFXAsset;

    bool destroyed = false;
    MeshCollider meshCollider;
    VisualEffect smokeFX;

    /// <summary>
    /// Event called when the component is destroyed
    /// </summary>
    [HideInInspector]
    public UnityEvent OnDestroyed = new UnityEvent();

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        UpdateHealth();
    }

    /// <summary>
    /// Apply damages depending on the relative position of the explosion, and it's radius.
    /// The damage are decreased the further the impact location is far for the object
    /// </summary>
    /// <param name="impactLocations"></param>
    /// <param name="radius"></param>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
    public void ApplyDamageAtLocation(Vector3[] impactLocations, float radius, float damage, GameObject instigator)
    {
        if (destroyed)
            return;

        // 1 Get the closest impact point
        float distance = radius * 2;
        Vector3 closestPoint = new Vector3(0, 0, 0);
        foreach (var point in impactLocations)
        {
            closestPoint = meshCollider.bounds.ClosestPoint(point);
            float newDistance = Vector3.Distance(closestPoint, point);
            if (newDistance < distance)
                distance = newDistance;
        }
        if (distance > radius)
            return;


        // Compute the damages
        float damagePercent = 1 - (distance / radius);
        float healthBefore = health;
        health -= Mathf.Clamp01(damagePercent) * damage;

        // If health dropped bellow 50, begin fire effect
        if (health < 50 && healthBefore >= 50)
            BeginSmoke(closestPoint);

        UpdateHealth();

        if (instigator)
        {
            // Tell the owning plane which was the instigator of the dammage (for exemple an ennemy)
            PlaneActor owningPlane = GetComponentInParent<PlaneActor>();
            if (owningPlane)
                owningPlane.LastDamageInstigator = instigator;
        }
    }

    /// <summary>
    /// Activate fire effect
    /// </summary>
    /// <param name="location"></param>
    void BeginSmoke(Vector3 location)
    {
        GameObject smokeContainer = new GameObject("SmokeContainer");
        smokeContainer.transform.parent = transform;
        smokeContainer.transform.localPosition = location;
        smokeFX = smokeContainer.AddComponent<VisualEffect>();
        smokeFX.visualEffectAsset = smokeFXAsset;
        smokeContainer.AddComponent<RocketBurstHandler>();

        GameObject smokeContainerParent = new GameObject("SmokeContainer");
        smokeContainerParent.transform.parent = transform.parent;
        smokeContainerParent.transform.position = transform.position;
        smokeFX = smokeContainerParent.AddComponent<VisualEffect>();
        smokeFX.visualEffectAsset = smokeFXAsset;
        smokeContainerParent.AddComponent<RocketBurstHandler>();
    }

    /// <summary>
    /// Notify the health have been changed
    /// </summary>
    void UpdateHealth()
    {
        if (health <= 0)
        {
            // If the component is not already detached, detach it
            if (!destroyed)
            {
                destroyed = true;

                // Make the speed of the component match it's parent velocity
                Rigidbody parentRB = GetComponentInParent<Rigidbody>();
                Vector3 currentVelocity = new Vector3(0, 0, 0);
                if (parentRB)
                    currentVelocity = parentRB.velocity;

                // Disable aerodynamics because we don't need it anymore.
                foreach (var aeroComp in GetComponentsInChildren<AerodynamicComponent>())
                    aeroComp.enabled = false;

                // Detach from parent
                transform.parent = null;

                // Add a landscape collider to make the object interract with the ground
                gameObject.AddComponent<LandscapeCollider>();

                // Retrieve the rigidbody of the part, or create a new one
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                if (!rb)
                    rb = gameObject.AddComponent<Rigidbody>();
                rb.velocity = currentVelocity;

                // Destroy this object in 60 seconds
                OnDestroyed.Invoke();
                Destroy(gameObject, 60);
            }
        }
    }
}

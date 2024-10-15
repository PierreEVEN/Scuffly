
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum PodItemType
{
    Rocket,
    Bomb,
    Maverick,
    Missile_IR,
    Missile_Rad,
    FuelTank
}

/// <summary>
/// A pod item can be attached to a weapon pod (under the wing) and can be used for missile, fuel tanks aso...
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class PodItem : MonoBehaviour, GPULandscapePhysicInterface
{
    /// <summary>
    /// The mass in kg of the object
    /// </summary>
    public float mass = 100;

    /// <summary>
    /// The kind of item (used by the weapon system)
    /// </summary>
    public PodItemType podItemType = PodItemType.FuelTank;

    /// <summary>
    /// Item destruction VFX
    /// </summary>
    public VisualEffectAsset explosionFx;

    /// <summary>
    /// The designed target (for missiles)
    /// </summary>
    [HideInInspector]
    public GameObject target;

    /// <summary>
    /// The rigidbody of the item (only available when detached)
    /// </summary>
    [HideInInspector]
    public Rigidbody physics;

    /// <summary>
    /// The object that was owning this pod item
    /// </summary>
    [HideInInspector]
    public GameObject owner;

    private void OnEnable() { GPULandscapePhysic.Singleton.AddListener(this); }
    private void OnDisable() { GPULandscapePhysic.Singleton.RemoveListener(this); }

    /// <summary>
    /// Deach the item from the pod, and enable it independant physic
    /// </summary>
    /// <param name="objectOwner"></param>
    /// <param name="initialSpeed"></param>
    /// <param name="target"></param>
    public virtual void Shoot(GameObject objectOwner, Vector3 initialSpeed, GameObject target)
    {
        this.owner = objectOwner;
        this.target = target;

        // detach from parent
        transform.parent = null;

        // add physics
        if (!physics)
        {
            physics = gameObject.AddComponent<Rigidbody>();
            physics.mass = mass;
            physics.freezeRotation = true;

            // Transmit owner's velocity
            physics.velocity = initialSpeed;
            physics.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        // Enable the aerodynamic simulation
        AerodynamicComponent aero = gameObject.GetComponent<AerodynamicComponent>();
        if (aero)
            aero.enabled = true;
    }

    /// <summary>
    /// Detect when the missile hit an other physic object
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // Destroy : if the contact object has a physic, make the vfx velocity match the target's one
        Rigidbody hitObjectPhysic = collision.gameObject.GetComponentInParent<Rigidbody>();
        if (hitObjectPhysic)
            Explode(hitObjectPhysic.velocity);
        else
            Explode(Vector3.zero);

        // Apply damage to target
        foreach (var comp in collision.gameObject.GetComponentsInChildren<DamageableComponent>())
            comp.ApplyDamageAtLocation(new Vector3[] { collision.contacts[0].point }, 2, 1000, owner);
    }

    public Vector2[] Collectpoints()
    {
        // Only one collision point is required (we don't need more)
        return new Vector2[1] { new Vector2(transform.position.x, transform.position.z) };
    }

    private void Update()
    {
        // Avoid the missile to explode on the plane that just fired it by disabling collision
        GetComponent<BoxCollider>().enabled = !owner || Vector3.Distance(owner.transform.position, transform.position) > 15;
    }

    public void OnPointsProcessed(float[] processedPoints)
    {
        // Explode if we hit the ground
        if (transform.position.y < processedPoints[0])
        {
            transform.position = new Vector3(transform.position.x, processedPoints[0], transform.position.z);
            Explode(Vector3.zero);
        }
    }

    /// <summary>
    /// Destruction of the item
    /// </summary>
    /// <param name="contactObjectVelocity"></param>
    public virtual void Explode(Vector3 contactObjectVelocity)
    {
        // Spawn an object containing the explosion FX, then destroy this one.
        // The vfx object is automatically destroyed after 20 seconds
        if (explosionFx)
        {
            GameObject fx = new GameObject("explosion_FX");
            VisualEffect vfx = fx.AddComponent<VisualEffect>();
            vfx.visualEffectAsset = explosionFx;
            vfx.SetVector3("Position", transform.position);
            vfx.SetVector3("Velocity", contactObjectVelocity * 0.8f);
            vfx.Play();
            foreach (var child in GetComponentsInChildren<VisualEffect>())
            {
                child.transform.parent = fx.transform;
                child.Stop();
            }
            Destroy(fx, 20);
        }
        Destroy(gameObject);
    }

}

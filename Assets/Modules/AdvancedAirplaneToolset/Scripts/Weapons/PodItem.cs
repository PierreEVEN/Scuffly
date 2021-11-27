
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

[RequireComponent(typeof(BoxCollider))]
public class PodItem : MonoBehaviour, GPULandscapePhysicInterface
{
    public static List<PodItem> PodItems = new List<PodItem>();

    public float mass = 100;
    public Mesh AerodynamicMesh;

    public PodItemType podItemType = PodItemType.FuelTank;

    public VisualEffectAsset explosionFx;

    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public Rigidbody physics;

    [HideInInspector]
    public GameObject owner;


    private void OnEnable()
    {
        PodItems.Add(this);
        GPULandscapePhysic.Singleton.AddListener(this);
    }

    private void OnDisable()
    {
        PodItems.Remove(this);
        GPULandscapePhysic.Singleton.RemoveListener(this);
    }

    public virtual void Shoot(GameObject objectOwner, Vector3 initialSpeed, Vector3 upVector, GameObject target)
    {
        this.owner = objectOwner;
        this.target = target;
        // detach from parent
        transform.parent = null;

        // add physics
        physics = gameObject.AddComponent<Rigidbody>();
        physics.mass = mass;
        physics.freezeRotation = true;

        // Avoid eventual collision with root
        transform.position += upVector * -GetComponent<BoxCollider>().bounds.size.y / 4;

        physics.velocity = initialSpeed;
        physics.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Add aerodynamics
        if (AerodynamicMesh)
        {
            AerodynamicComponent aero = gameObject.AddComponent<AerodynamicComponent>();
            aero.meshOverride = AerodynamicMesh;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody hitObjectPhysic = collision.gameObject.GetComponentInParent<Rigidbody>();
        if (hitObjectPhysic)
            Explode(hitObjectPhysic.velocity);
        else
            Explode(Vector3.zero);

        foreach (var comp in collision.gameObject.GetComponentsInChildren<DamageableComponent>())
            comp.ApplyDamageAtLocation(new Vector3[] { collision.contacts[0].point }, 2, 1000);
    }

    public Vector2[] Collectpoints()
    {
        return new Vector2[1] { new Vector2(transform.position.x, transform.position.z) };
    }

    public void OnPointsProcessed(float[] processedPoints)
    {
        if (transform.position.y < processedPoints[0])
        {
            transform.position = new Vector3(transform.position.x, processedPoints[0], transform.position.z);
            Explode(Vector3.zero);
        }
    }
    public virtual void Explode(Vector3 contactObjectVelocity)
    {
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

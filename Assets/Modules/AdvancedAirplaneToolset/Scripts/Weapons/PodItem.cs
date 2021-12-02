
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

/**
 * Objet attacheable à un pod d'armement (placé sous les ailes d'un avion)
 */
[RequireComponent(typeof(BoxCollider))]
public class PodItem : MonoBehaviour, GPULandscapePhysicInterface
{
    // Liste des items pouvant etre attaches (//@TODO : faire un menu pour selectioner l'armement / pour l'instant spawn automatiquement le premier de la liste)
    public static List<PodItem> PodItems = new List<PodItem>();

    // Masse de l'objet
    public float mass = 100;
    // Type de l'item attaché (utilisé par le systeme d'armement)
    public PodItemType podItemType = PodItemType.FuelTank;

    // VFX a l'impact
    public VisualEffectAsset explosionFx;

    // Cible enregistrée (uniquement pour les missiles et certaines bombes)
    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public Rigidbody physics;

    // Auteur du tir
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

    // Detache l'objet du pod et met en place une physique indépendante de l'avion porteur
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
        AerodynamicComponent aero = gameObject.GetComponent<AerodynamicComponent>();
        if (aero)
            aero.enabled = true;
    }

    // Detecte un impact avec un autre objet
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
        // Un seul point est utilisé pour la detection de collision avec le landscape (pas besoin de plus)
        return new Vector2[1] { new Vector2(transform.position.x, transform.position.z) };
    }

    public void OnPointsProcessed(float[] processedPoints)
    {
        if (transform.position.y < processedPoints[0])
        {
            // Si l'objet est passé sous le sol, le detruit instantanément
            transform.position = new Vector3(transform.position.x, processedPoints[0], transform.position.z);
            Explode(Vector3.zero);
        }
    }
    // Detruit l'objet, et declanche les VFX d'explosions
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

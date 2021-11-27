using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

/**
 * Un DamageableComponent est un element de l'avion qui pourra potentiellement etre détaché de celui-ci en cas de choc ou autre.
 */
[RequireComponent(typeof(MeshCollider))]
public class DamageableComponent : MonoBehaviour
{
    // Niveau de vie actuel du component
    public float health = 100;
    // VFX d'incendis
    public VisualEffectAsset smokeFXAsset;

    bool destroyed = false;
    MeshCollider meshCollider;
    VisualEffect smokeFX;

    // Event appelé au moment où le component est détaché de son parent (dommages critiques)
    public UnityEvent OnDestroyed = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        UpdateHealth();
    }

    // Applique des dommages a partir de points d'impacts. Les degars seront calcules en fonction du point d'impact, et du rayon.
    public void ApplyDamageAtLocation(Vector3[] impactLocations, float radius, float damage)
    {
        if (destroyed)
            return;

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

        float damagePercent = 1 - (distance / radius);
        float healthBefore = health;
        health -= Mathf.Clamp01(damagePercent) * damage;
        // Commence les effets de fumee dès qu'on passe en dessous des 50 de vie
        if (health < 50 && healthBefore >= 50)
            BeginSmoke(closestPoint);
        UpdateHealth();
    }

    // Debut d'un incendis
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

    // A appeler en cas de changement d'etat de la vie de l'avion
    void UpdateHealth()
    {
        if (health <= 0)
        {
            // Si le component n'est pas encore détaché, on le detache.
            if (!destroyed)
            {
                destroyed = true;

                Rigidbody parentRB = GetComponentInParent<Rigidbody>();
                Vector3 currentVelocity = new Vector3(0, 0, 0);
                if (parentRB)
                    currentVelocity = parentRB.velocity;

                transform.parent = null;

                gameObject.AddComponent<LandscapeCollider>();

                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                if (!rb)
                    rb = gameObject.AddComponent<Rigidbody>();
                rb.velocity = currentVelocity;
                foreach (var aeroComp in GetComponentsInChildren<AerodynamicComponent>())
                {
                    aeroComp.enabled = false;
                }

                OnDestroyed.Invoke();
                Destroy(gameObject, 60);
            }
        }
    }
}

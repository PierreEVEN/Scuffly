using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[RequireComponent(typeof(MeshCollider))]
public class DamageableComponent : MonoBehaviour
{
    public float health = 100;

    bool destroyed = false;
    MeshCollider meshCollider;

    public VisualEffectAsset smokeFXAsset;
    VisualEffect smokeFX;

    public UnityEvent OnDestroyed = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        UpdateHealth();
    }

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
        if (health < 50 && healthBefore >= 50)
            BeginSmoke(closestPoint);
        UpdateHealth();
    }

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

    void UpdateHealth()
    {
        if (health <= 0)
        {
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

    // Update is called once per frame
    void Update()
    {
    }
}

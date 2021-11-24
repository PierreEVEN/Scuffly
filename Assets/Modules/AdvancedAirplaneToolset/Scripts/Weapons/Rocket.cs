using UnityEngine;
using UnityEngine.VFX;

/*
 * @Autor : Leo
 * 
 * Armement de type missile attachable a un pod d'armement
 * 
 */
[RequireComponent(typeof(BoxCollider))]
public class Rocket : MonoBehaviour, GPULandscapePhysicInterface
{
    public float Acceleration = 100;
    public float Endurance = 5;
    public float initialForce = 20;

    public float mass = 85;

    private float depopDelay = 10;
    private bool destroyed = false;
    Rigidbody rb;
    BoxCollider weaponCollider;
    VisualEffect vfx;
    public VisualEffectAsset explosionFx;
    private VisualEffect explCompFx;
    GameObject target;
    Vector3 hitVelocity = new Vector3(0, 0, 0);

    public Mesh physicMesh;

    private void OnEnable()
    {
        GPULandscapePhysic.Singleton.AddListener(this);
    }

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        vfx = GetComponentInChildren<VisualEffect>();
        if (vfx && transform.parent != null)
        {
            vfx.enabled = false;
        }
    }

    private void OnDisable()
    {
        GPULandscapePhysic.Singleton.RemoveListener(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody collRb = collision.gameObject.GetComponentInParent<Rigidbody>();
        if (collRb)
            hitVelocity = collRb.velocity;

        Detonate();

        foreach (var comp in collision.gameObject.GetComponentsInChildren<DamageableComponent>())
        {
            comp.ApplyDamageAtLocation(new Vector3[] { collision.contacts[0].point }, 2, 1000);
        }
    }

    void Detonate()
    {
        if (destroyed)
            return;
        if (rb)
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.freezeRotation = true;
        }
        Acceleration = 0;
        vfx.enabled = false;
        GameObject fxObj = new GameObject();
        fxObj.transform.parent = transform;
        fxObj.transform.localPosition = new Vector3(0, 0, 0);
        explCompFx = fxObj.AddComponent<VisualEffect>();
        explCompFx.visualEffectAsset = explosionFx;
        explCompFx.SetVector3("Position", transform.position);
        explCompFx.SetVector3("Velocity", hitVelocity * 0.8f);
        explCompFx.Play();
        destroyed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed)
        {
            depopDelay -= Time.deltaTime;
            if (depopDelay <= 0)
            {
                Destroy(gameObject);
            }
        }
    }


    private void FixedUpdate()
    {
        if (rb && !destroyed)
        {

            Endurance -= Time.deltaTime;
            if (Endurance > 0)
            {
                float step = Acceleration * Time.fixedDeltaTime; // calcule la distance que le missile va parcourir � la prochaine �tape
                rb.velocity += transform.forward * step;
            }
            else if (vfx)
                //vfx.enabled = false;
                ;

            Vector3 rocketRelativeVelocity = transform.InverseTransformDirection(rb.velocity);
            rocketRelativeVelocity.z = 0;

            Vector3 rocketRadialVelocity = transform.TransformDirection(rocketRelativeVelocity);


            Vector3 targetPosition = target.transform.position;

            float distanceToTarget = (targetPosition - transform.position).magnitude;
            float timeBeforeImpact = distanceToTarget / rb.velocity.magnitude;

            Vector3 targetVelocity = Vector3.zero;
            var targetRb = target.GetComponent<Rigidbody>();
            if (targetRb)
            {
                targetVelocity = targetRb.velocity;
            }


            Vector3 correctedTargetPosition = targetPosition + targetVelocity * timeBeforeImpact - rocketRadialVelocity * timeBeforeImpact * 0.1f;


            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((correctedTargetPosition - transform.position).normalized, new Vector3(0, 1, 0)), 350);

        }
    }


    public void Shoot(Vector3 initialSpeed, GameObject target)
    {
        this.target = target;
        if (!target)
            return;

        transform.parent = null;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = mass;

        AerodynamicComponent aero = gameObject.AddComponent<AerodynamicComponent>();
        aero.meshOverride = physicMesh;

        if (weaponCollider)
            transform.position += transform.up * -weaponCollider.bounds.size.y / 4;
        rb.velocity = initialSpeed + transform.up * -initialForce;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        if (vfx && vfx.visualEffectAsset)
        {
            vfx.enabled = true;
        }
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
            Detonate();
        }
    }
}

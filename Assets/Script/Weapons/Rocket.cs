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

    private float depopDelay = 10;
    private bool destroyed = false;
    Rigidbody rb;
    BoxCollider weaponCollider;
    VisualEffect vfx;
    public VisualEffectAsset explosionFx;
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        vfx = GetComponentInChildren<VisualEffect>();
        if (vfx && transform.parent != null)
        {
            vfx.enabled = false;
        }
    }

    private void OnEnable()
    {
        GPULandscapePhysic.Singleton.AddListener(this);
    }
    private void OnDisable()
    {
        GPULandscapePhysic.Singleton.RemoveListener(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
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
        VisualEffect explfx = fxObj.AddComponent<VisualEffect>();
        explfx.visualEffectAsset = explosionFx;
        explfx.Play();
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

        if (rb )
        {
            Endurance -= Time.deltaTime;
            if (Endurance > 0)
            {
                float step = Acceleration * Time.deltaTime; // calcule la distance que le missile va parcourir à la prochaine étape
                rb.velocity += transform.forward * step;
            }
            else if (vfx)
                vfx.enabled = false;

            float dragLimiter = 0.9f;
            int turningSpeed = 60;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.transform.position - transform.position).normalized, new Vector3(0, 1, 0)), turningSpeed * Time.deltaTime);

            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            localVelocity.x *= dragLimiter;
            localVelocity.y *= dragLimiter;
            rb.velocity = transform.TransformDirection(localVelocity);

        }
    }



    public void Shoot(Vector3 initialSpeed, GameObject target)
    {
        this.target = target;
        if (!target)
            return;

        transform.parent = null;
        rb = gameObject.AddComponent<Rigidbody>();
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
        if (transform.position.y < processedPoints[0]) Detonate();
    }
}

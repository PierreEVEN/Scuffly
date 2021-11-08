using UnityEngine;
using UnityEngine.VFX;

/*
 * Armement de type missile attachable a un pod d'armement
 * 
 * @TODO : pas fini
 */
[RequireComponent(typeof(BoxCollider))]
public class Rocket : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        vfx = GetComponentInChildren<VisualEffect>();
        if (vfx)
        {
            vfx.enabled = false;
        }
    }

    int collisionID;
    private void OnEnable()
    {
        collisionID = GPULandscapePhysic.Singleton.AddCollisionItem(new Vector2[1] { new Vector2(transform.position.x, transform.position.z) });
        GPULandscapePhysic.Singleton.OnPreProcess.AddListener(OnPreprocessPhysic);
        GPULandscapePhysic.Singleton.OnProcessed.AddListener(OnProcessedPhysic);
    }
    private void OnDisable()
    {
        GPULandscapePhysic.Singleton.RemoveCollisionItem(collisionID);
        GPULandscapePhysic.Singleton.OnPreProcess.RemoveListener(OnPreprocessPhysic);
        GPULandscapePhysic.Singleton.OnProcessed.RemoveListener(OnProcessedPhysic);
    }

    void OnPreprocessPhysic()
    {
        GPULandscapePhysic.Singleton.UpdateSourcePoints(collisionID, new Vector2[1] { new Vector2(transform.position.x, transform.position.z) });
    }
    void OnProcessedPhysic()
    {
        var fsxData = GPULandscapePhysic.Singleton.GetPhysicData(collisionID);
        if (transform.position.y < fsxData[0]) Detonate();
    }


    private void OnCollisionEnter(Collision collision)
    {
        Detonate();
    }

    void Detonate()
    {
        if (destroyed)
            return;
        rb.velocity = new Vector3(0, 0, 0);
        rb.freezeRotation = true;
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

        if (rb && Endurance > 0)
        {
            Endurance -= Time.deltaTime;
            rb.velocity += transform.forward * Time.deltaTime * Acceleration;
        }
        else
            if (vfx)
            vfx.enabled = false;
    }

    public void Shoot(Vector3 initialSpeed)
    {
        transform.parent = null;
        rb = gameObject.AddComponent<Rigidbody>();
        transform.position += transform.up * -weaponCollider.bounds.size.y / 2;
        rb.velocity = initialSpeed + transform.up * -initialForce;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        if (vfx && vfx.visualEffectAsset)
        {
            vfx.enabled = true;
        }
    }
}

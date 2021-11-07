using UnityEngine;
using UnityEngine.VFX;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > 20)
        {
            Debug.Log("boom");
            rb.velocity = new Vector3(0, 0, 0);
            GameObject fxObj = new GameObject();
            fxObj.transform.position = transform.position;
            VisualEffect explfx = fxObj.AddComponent<VisualEffect>();
            explfx.visualEffectAsset = explosionFx;
            explfx.Play();
            destroyed = true;
        }
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

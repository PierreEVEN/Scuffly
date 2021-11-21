using UnityEngine;
using UnityEngine.VFX;


/*
 * Component qui recupere les forces physique au point indique, et transmet ces informations a un systeme de particule (ou autre)
 */
public class PhysicsFeedbackAtPoint : MonoBehaviour
{
    Rigidbody rb;
    VisualEffect fxTest;

    Vector3 currentAcceleration;
    Vector3 smoothedAcceleration;

    Vector3 lastVelocity;

    public Vector3 Acceleration
    {
        get { return smoothedAcceleration; }
    }

    public Vector3 GForce
    {
        get { return smoothedAcceleration / -9.81f + new Vector3(0, -1, 0); }
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        if (!rb)
        {
            rb = GetComponentInParent<Rigidbody>();
            lastVelocity = rb.velocity;
        }
        if (!rb)
            Debug.LogError("cannot find rigidbody");

        // Calcule la force d'acceleration
        Vector3 relativeVelocity = transform.InverseTransformDirection(rb.GetPointVelocity(transform.position));
        currentAcceleration = (relativeVelocity - lastVelocity) / Time.deltaTime;

        smoothedAcceleration = new Vector3(
            Mathf.Lerp(smoothedAcceleration.x, relativeVelocity.x, Time.deltaTime * 10),
            Mathf.Lerp(smoothedAcceleration.y, relativeVelocity.y, Time.deltaTime * 10),
            Mathf.Lerp(smoothedAcceleration.z, relativeVelocity.z, Time.deltaTime * 10));

        lastVelocity = rb.velocity;

        if (!fxTest)
            fxTest = GetComponent<VisualEffect>();
        if (!fxTest)
            return;

        if (fxTest.HasFloat("Acceleration"))
        {
            fxTest.SetFloat("Acceleration", Acceleration.magnitude);
        }

        if (fxTest.HasFloat("UpVelocity"))
        {
            fxTest.SetFloat("UpVelocity", relativeVelocity.y);
        }
    }
}

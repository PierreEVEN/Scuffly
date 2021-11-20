using UnityEngine;
using UnityEngine.VFX;


/*
 * Component qui recupere les forces physique au point indique, et transmet ces informations a un systeme de particule (ou autre)
 */
public class PhysicsFeedbackAtPoint : MonoBehaviour
{
    Rigidbody rb;
    VisualEffect fxTest;

    float currentAcceleration = 0;

    Vector3 lastVelocity;

    public float Acceleration
    {
        get { return currentAcceleration; }
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
        Vector3 currentVelocity = rb.GetPointVelocity(transform.position);
        currentAcceleration = (currentVelocity - lastVelocity + new Vector3(0, -9.81f /* gravity */, 0) * Time.deltaTime).magnitude / Time.deltaTime;

        // Calcule la vitesse relative du component
        Vector3 relativeVelocity = transform.InverseTransformDirection(rb.velocity);

        lastVelocity = rb.velocity;

        if (!fxTest)
            fxTest = GetComponent<VisualEffect>();
        if (!fxTest)
            return;

        if (fxTest.HasFloat("Acceleration"))
        {
            fxTest.SetFloat("Acceleration", currentAcceleration);
        }

        if (fxTest.HasFloat("UpVelocity"))
        {
            fxTest.SetFloat("UpVelocity", relativeVelocity.y);
        }
    }
}
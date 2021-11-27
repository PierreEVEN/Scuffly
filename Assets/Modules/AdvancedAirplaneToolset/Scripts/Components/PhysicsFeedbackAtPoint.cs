using UnityEngine;
using UnityEngine.VFX;


/*
 * Component qui recupere les forces physique au point indiqué, et transmet ces informations a un systeme de particule (ou autre)
 */
public class PhysicsFeedbackAtPoint : PlaneComponent
{
    VisualEffect fx;

    Vector3 smoothedAcceleration;
    Vector3 lastVelocity;

    // Acceleration lissee (espace local)
    public Vector3 Acceleration
    {
        get { return smoothedAcceleration; }
    }

    // Forces de G appliquees au point transform.position (espace local)
    public Vector3 GForce
    {
        get { return smoothedAcceleration / -9.81f + new Vector3(0, -1, 0); }
    }

    private void OnEnable()
    {
        lastVelocity = Physics.velocity;
    }

    void FixedUpdate()
    {
        // Calcule la force d'acceleration relative
        Vector3 relativeVelocity = transform.InverseTransformDirection(Physics.GetPointVelocity(transform.position));

        smoothedAcceleration = new Vector3(
            Mathf.Lerp(smoothedAcceleration.x, relativeVelocity.x, Time.deltaTime * 10),
            Mathf.Lerp(smoothedAcceleration.y, relativeVelocity.y, Time.deltaTime * 10),
            Mathf.Lerp(smoothedAcceleration.z, relativeVelocity.z, Time.deltaTime * 10));

        lastVelocity = relativeVelocity;

        // transmet eventuellement les informations a un systeme de particule
        if (!fx)
            fx = GetComponent<VisualEffect>();
        if (!fx)
            return;

        if (fx.HasFloat("Acceleration"))
        {
            fx.SetFloat("Acceleration", Acceleration.magnitude);
        }

        if (fx.HasFloat("UpVelocity"))
        {
            fx.SetFloat("UpVelocity", relativeVelocity.y);
        }
    }
}

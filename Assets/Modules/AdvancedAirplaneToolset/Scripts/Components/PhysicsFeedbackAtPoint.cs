using UnityEngine;
using UnityEngine.VFX;


/// <summary>
/// This component compute the physical force at a given point
/// </summary>
public class PhysicsFeedbackAtPoint : PlaneComponent
{
    /// <summary>
    /// The component can tell a fx system the values he got
    /// </summary>
    VisualEffect fx;

    /// <summary>
    /// The acceleration result is smoothed too avoid imprecision problems
    /// </summary>
    Vector3 smoothedAcceleration;

    /// <summary>
    /// Retrieve the smoothed acceleration in m.s
    /// </summary>
    public Vector3 Acceleration
    {
        get { return smoothedAcceleration; }
    }

    /// <summary>
    /// Retrieve the G forces at this transform
    /// </summary>
    public Vector3 GForce
    {
        get { return smoothedAcceleration / -9.81f + new Vector3(0, -1, 0); }
    }

    void FixedUpdate()
    {
        // Get relative velocity at transform
        Vector3 relativeVelocity = transform.InverseTransformDirection(Physics.GetPointVelocity(transform.position));

        // Interpolate the acceleration with last one to smooth it
        smoothedAcceleration = new Vector3(
            Mathf.Lerp(smoothedAcceleration.x, relativeVelocity.x, Time.deltaTime * 10),
            Mathf.Lerp(smoothedAcceleration.y, relativeVelocity.y, Time.deltaTime * 10),
            Mathf.Lerp(smoothedAcceleration.z, relativeVelocity.z, Time.deltaTime * 10));

        // Send the computed data to a FX system (optionnal)
        if (!fx)
            fx = GetComponent<VisualEffect>();
        if (!fx)
            return;

        if (fx.HasFloat("Acceleration"))
            fx.SetFloat("Acceleration", Acceleration.magnitude);

        if (fx.HasFloat("UpVelocity"))
            fx.SetFloat("UpVelocity", relativeVelocity.y);
    }
}

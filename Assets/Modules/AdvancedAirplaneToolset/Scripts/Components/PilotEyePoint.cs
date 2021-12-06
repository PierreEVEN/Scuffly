using UnityEngine;

/// <summary>
/// View point of the pilot. Simulate camera shakes and acceleration effects
/// </summary>
[RequireComponent(typeof(PhysicsFeedbackAtPoint))]
public class PilotEyePoint : MonoBehaviour
{    
    PhysicsFeedbackAtPoint physics;

    /// <summary>
    /// The camera is slighty moved in 3 dimensions depending on the perceived accelerations 
    /// </summary>
    float GOffsetIntensity = 0.0004f;
    float GForceValue = 0;


    void OnEnable()
    {
        physics = GetComponent<PhysicsFeedbackAtPoint>();
    }

    /// <summary>
    /// Compute the camera location
    /// //@TODO add camera shakes
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCameraLocation()
    {
        return transform.position +
            (transform.right * physics.Acceleration.x * 4.5f + transform.up * physics.Acceleration.y * 1.5f + transform.forward * physics.Acceleration.z * 0.1f) 
            * -1 * GOffsetIntensity;
    }

    private void Update()
    {
        float inputForce = physics.GForce.y / 2.5f;
        float newForce = Mathf.Max(0,Mathf.Abs(inputForce) - 1);
        GForceValue = GForceValue + Mathf.Clamp(newForce * Mathf.Sign(inputForce) - GForceValue, -Time.deltaTime * 0.2f, Time.deltaTime * 0.2f);
        if (newForce > Mathf.Abs(GForceValue))
            GForceValue = newForce * Mathf.Sign(inputForce);
        if (Mathf.Abs(GForceValue) > 1 && Mathf.Abs(GForceValue) <= 1)
            GForceValue *= 2;
    }

    /// <summary>
    /// Get gforce effect level
    /// (between -10 and 10) for negative and positive effect. Mainly used for post processing
    /// </summary>
    /// <returns></returns>
    public float GetGforceEffect()
    {
        return GForceValue;
    }

}

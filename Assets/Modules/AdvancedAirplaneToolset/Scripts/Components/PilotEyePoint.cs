using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsFeedbackAtPoint))]
public class PilotEyePoint : MonoBehaviour
{    
    PhysicsFeedbackAtPoint physics;

    float GOffsetIntensity = 0.0004f;

    float GForceValue = 0;


    // Start is called before the first frame update
    void OnEnable()
    {
        physics = GetComponent<PhysicsFeedbackAtPoint>();
    }

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

        float OldG = GForceValue;

        GForceValue = GForceValue + Mathf.Clamp(newForce * Mathf.Sign(inputForce) - GForceValue, -Time.deltaTime * 0.2f, Time.deltaTime * 0.2f);

        if (newForce > Mathf.Abs(GForceValue))
            GForceValue = newForce * Mathf.Sign(inputForce);

        if (Mathf.Abs(GForceValue) > 1 && Mathf.Abs(GForceValue) <= 1)
            GForceValue *= 2;
    }

    public float GetGforceEffect()
    {
        return GForceValue;
    }

}

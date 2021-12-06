
using UnityEngine;

/// <summary>
/// Handle landing lights of a retractable gear
/// </summary>
[RequireComponent(typeof(Light))]
public class LandingLightManager : PlaneComponent
{
    /// <summary>
    /// The intensity of the light the component is attached to
    /// </summary>
    private float initialValue;

    private void OnEnable()
    {
        initialValue = GetComponent<Light>().intensity;
    }

    private void OnDisable()
    {
        GetComponent<Light>().intensity = initialValue;
    }

    void Update()
    {
        // Update the light level
        //@TODO : improve performances by using events instead. (weird bug that make event not working to fix before)
        GetComponent<Light>().intensity = (Plane.GetCurrentPower() > 90 && !Plane.RetractGear && Plane.LandingLights) ? initialValue : 0;
    }
}

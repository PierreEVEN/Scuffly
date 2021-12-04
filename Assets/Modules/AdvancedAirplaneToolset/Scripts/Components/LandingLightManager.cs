
using UnityEngine;

public class LandingLightManager : PlaneComponent
{
    float initialValue;
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
        GetComponent<Light>().intensity = (Plane.GetCurrentPower() > 90 && !Plane.RetractGear && Plane.LandingLights) ? initialValue : 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle the flood lights of the interior.
/// Each light attached to this component is considered as a flood light
/// </summary>
public class FloodLightController : PlaneComponent
{
    /// <summary>
    /// List of found lights, and their base intensity
    /// </summary>
    Light[] lights;
    float[] initialIntensities;

    private void OnEnable()
    {
        lights = GetComponentsInChildren<Light>();
        initialIntensities = new float[lights.Length];
        for (int i = 0; i < lights.Length; ++i)
            initialIntensities[i] = lights[i].intensity;
    }

    private void OnDisable()
    {
        // Set the base intensity for each light
        for (int i = 0; i < lights.Length; ++i)
            lights[i].intensity = initialIntensities[i];
    }

    void Update()
    {
        // Update the light intensity, depending on the power of the plane.
        //@TODO : improve performances by using events instead. (weird bug that make event not working to fix before)
        for (int i = 0; i < lights.Length; ++i)
            lights[i].intensity = Plane.CockpitFloodLights * initialIntensities[i] * Mathf.Clamp01(Plane.GetCurrentPower());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle a group of anti-collision lights on the plane.
/// Each light require a minimum power to work.
/// The component will considere as anti-collision light all the lights in it's children
/// </summary>
public class AntiColLightManager : PlaneComponent
{
    /// <summary>
    /// List of lights used and their base intensities
    /// </summary>
    Light[] lights;
    float[] initialIntensities;

    private void OnEnable()
    {
        // Collect lights, and store their intensities
        lights = GetComponentsInChildren<Light>();
        initialIntensities = new float[lights.Length];
        for (int i = 0; i < lights.Length; ++i)
            initialIntensities[i] = lights[i].intensity;
    }

    private void OnDisable()
    {
        // Reset the intensity to their original value
        for (int i = 0; i < lights.Length; ++i)
            lights[i].intensity = initialIntensities[i];
    }

    void Update()
    {
        // Update the lights level
        for (int i = 0; i < lights.Length; ++i)
            lights[i].intensity = Plane ? Plane.PositionLight * initialIntensities[i] * Mathf.Clamp01(Plane.GetCurrentPower()) : 0;
    }
}

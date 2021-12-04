using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodLightController : PlaneComponent
{
    Light[] lights;
    float[] initialIntensities;

    private void OnEnable()
    {
        lights = GetComponentsInChildren<Light>();
        initialIntensities = new float[lights.Length];
        for (int i = 0; i < lights.Length; ++i)
        {
            initialIntensities[i] = lights[i].intensity;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < lights.Length; ++i)
        {
            lights[i].intensity = initialIntensities[i];
        }
    }

    void Update()
    {
        for (int i = 0; i < lights.Length; ++i)
        {
            lights[i].intensity = Plane.CockpitFloodLights * initialIntensities[i] * Mathf.Clamp01(Plane.GetCurrentPower());
        }
    }
}

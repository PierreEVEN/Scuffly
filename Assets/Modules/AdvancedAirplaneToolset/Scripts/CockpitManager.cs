using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitManager : PlaneComponent
{
    private void OnEnable()
    {
        Plane.OnGlobalPowerChanged.AddListener(OnGlobalPowerChanged);
        OnGlobalPowerChanged();
    }

    private void OnDisable()
    {
        Plane.OnGlobalPowerChanged.RemoveListener(OnGlobalPowerChanged);
    }

    void OnGlobalPowerChanged()
    {
        SetGlobalIntensity(Plane.GetCurrentPower() > 10 ? Mathf.Clamp01((Plane.GetCurrentPower() - 10) / 50) + 0.2f : 0.2f);
    }

    void SetGlobalIntensity(float intensity)
    {
        Color color = new Color(intensity, Mathf.Pow(intensity, 1.2f), intensity, 1);
        foreach (var metter in GetComponentsInChildren<MetterRenderer>())
        {
            metter.LineColor = color;
            metter.UpdateMaterialProperties();
            foreach (var canvas in metter.gameObject.GetComponentsInChildren<CanvasGroup>())
            {
                canvas.alpha = intensity;
            }
        }
    }
}

using AK.Wwise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Systeme de gestion du cockpit : gere notament l'intensite des lumieres
public class CockpitManager : PlaneComponent
{
    private void OnEnable()
    {
        Plane.OnGlobalPowerChanged.AddListener(OnGlobalPowerChanged);
        OnGlobalPowerChanged();
    }

    public AK.Wwise.Event PlayCockpitSound;
    public AK.Wwise.Event StopCockpitSound;
    bool isPLayingAudio = false;

    private void OnDisable()
    {
        Plane.OnGlobalPowerChanged.RemoveListener(OnGlobalPowerChanged);
    }

    void OnGlobalPowerChanged()
    {
        SetGlobalIntensity(Plane.GetCurrentPower() > 10 ? Mathf.Clamp01((Plane.GetCurrentPower() - 10) / 50) + 0.2f : 0.2f);
        if (Plane.MainPower && !isPLayingAudio)
        {
            isPLayingAudio = true;
            PlayCockpitSound.Post(gameObject);
        }
        if (!Plane.MainPower && isPLayingAudio)
        {
            isPLayingAudio = false;
            StopCockpitSound.Post(gameObject);
        }
    }

    // Met a jour les lumieres interieurs (en cas de changement de puissance electrique)
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

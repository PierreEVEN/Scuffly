using UnityEngine;

/// <summary>
/// Main class of the cockpit.
/// Also handle all the interior lights
/// </summary>
public class CockpitManager : PlaneComponent
{
    private void OnEnable()
    {
        Plane.OnGlobalPowerChanged.AddListener(OnGlobalPowerChanged);
        OnGlobalPowerChanged();
    }

    /// <summary>
    /// The main power switch enable some ventilators, so we play some noise :)
    /// </summary>
    public AK.Wwise.Event PlayCockpitSound;
    public AK.Wwise.Event StopCockpitSound;
    bool isPlayingAudio = false;

    private void OnDisable()
    {
        Plane.OnGlobalPowerChanged.RemoveListener(OnGlobalPowerChanged);
    }

    /// <summary>
    /// Update components that require plane power
    /// </summary>
    void OnGlobalPowerChanged()
    {
        // Update metter intensity
        SetAllMeterIntensity(Plane.GetCurrentPower() > 10 ? Mathf.Clamp01((Plane.GetCurrentPower() - 10) / 50) + 0.2f : 0.2f);

        // Update ventilator sound
        if (Plane.MainPower && !isPlayingAudio)
        {
            isPlayingAudio = true;
            PlayCockpitSound.Post(gameObject);
        }
        if (!Plane.MainPower && isPlayingAudio)
        {
            isPlayingAudio = false;
            StopCockpitSound.Post(gameObject);
        }
    }

    /// <summary>
    /// Update the lights level of metters
    /// //@TODO : improve the interior light system to use an emissive map instead
    /// </summary>
    /// <param name="intensity"></param>
    void SetAllMeterIntensity(float intensity)
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

using UnityEngine;

/// <summary>
/// An APU is an intermediate power generator that is required to start the engine, and to have a minimum of power when the main engine is not enabled
/// </summary>
[RequireComponent(typeof(AudioEngine))]
public class APU : PlaneComponent, IPowerProvider
{
    /// <summary>
    /// Startup and shutdown duration of the APU in seconds
    /// </summary>
    public float StartupDuration = 14;
    public float ShutdownDuration = 12;

    /// <summary>
    /// State of the APU : 
    /// If the main engine is generating more power than the current APU for 10 seconds, it shut down itself automatically
    /// </summary>
    private float currentStartupPercent = 0.0f;
    private bool apuEnabled = false;
    private float enoughPowerTimer = 0;

    /// <summary>
    /// Audio system of the APU
    /// </summary>
    AudioEngine audioEngine;

    void OnEnable()
    {
        // Detect power changes
        Plane.OnApuChange.AddListener(UpdateState);
        Plane.OnPowerSwitchChanged.AddListener(UpdateState);
        Plane.RegisterPowerProvider(this);

        // If the plane is started with APU ON : make it fully started
        if (Plane.initialApuSwitch)
        {
            currentStartupPercent = 1;
            apuEnabled = true;
        }
        UpdateState();

        audioEngine = GetComponent<AudioEngine>();
    }

    private void OnDisable()
    {
        Plane.OnApuChange.RemoveListener(UpdateState);
        Plane.OnPowerSwitchChanged.RemoveListener(UpdateState);
        apuEnabled = false;
    }

    private void OnGUI()
    {
        // Debug
        if (!Plane.EnableDebug)
            return;
        GUILayout.BeginArea(new Rect(200, 0, 200, 100));
        GUILayout.Label("APU Enabled : " + apuEnabled);
        GUILayout.Label("APU IN : " + Plane.GetCurrentPower() + " / 5");
        GUILayout.Label("APU OUT : " + currentStartupPercent * 40);
        GUILayout.EndArea();
    }

    /// <summary>
    /// Update the APU State.
    /// The APU require a minimum of 5 Kva to start, and is automatically stopped when the total power exceend 880 Kva
    /// </summary>
    void UpdateState()
    {
        if (Plane.EnableAPU && Plane.GetCurrentPower() > 5 && enoughPowerTimer < 10) // If the main engine provide enough power for 10s : stop the APU
            apuEnabled = true;
        else
        {
            enoughPowerTimer = 0;
            apuEnabled = false;
            Plane.EnableAPU = false;
        }
    }


    void Update()
    {
        // Handle the progressive startup of the APU (Constant interpolation)
        currentStartupPercent = Mathf.Clamp(currentStartupPercent + (apuEnabled ? Time.deltaTime / StartupDuration : -Time.deltaTime / ShutdownDuration), 0, 1);

        audioEngine.SetStartPercent(currentStartupPercent);

        // Check if the APU should be stopped
        if (apuEnabled)
        {
            if (Plane.GetCurrentPower() > 80)
                enoughPowerTimer += Time.deltaTime;
            else
                enoughPowerTimer = 0;
            UpdateState();
        }

    }

    /// <summary>
    /// Interface IPowerProvider : Send the produced energy to the plane
    /// </summary>
    public float GetPower()
    {
        return currentStartupPercent * 40;
    }
}

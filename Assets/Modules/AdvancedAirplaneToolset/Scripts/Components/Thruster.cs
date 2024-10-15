using AK.Wwise;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Physic thruster of the plane.
/// </summary>
[RequireComponent(typeof(AudioEngine))]
public class Thruster : PlaneComponent, IPowerProvider
{
    /// <summary>
    /// Curve of acceleration (in percent). It make the force provided not follow the input linearly
    /// </summary>
    public AnimationCurve ThrustPercentCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.7f, 0.6f), new Keyframe(1, 1.1f) });

    /// <summary>
    /// Thrust force in Newtons provided by the thruster. The thrust is not linear and is influenced by the current intake air speed
    /// </summary>
    public AnimationCurve ThrustForceCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 3000000.0f), new Keyframe(300, 8000000.0f) });

    /// <summary>
    /// Duration of startup and shutdown
    /// </summary>
    public float engineStartupDuration = 39;
    public float engineShutdownDuration = 39;

    /// <summary>
    /// Current startup state (between 0 and 1)
    /// </summary>
    [HideInInspector]
    public float engineStartupPercent = 0;

    /// <summary>
    /// The force (in input level) provided at iddle
    /// </summary>
    public float idleEngineThrustPercent = 0.001f;

    /// <summary>
    /// The acceleration speed of the engine (when fully started)
    /// </summary>
    public float thrustPercentAcceleration = 0.4f;
    private float throttleDesiredPercent = 0;
    private float throttleCurrentPercent = 0;

    /// <summary>
    /// The object containing the thrust particle
    /// </summary>
    public GameObject VFXObject;

    /// <summary>
    /// The object containing the light emitter
    /// </summary>
    public GameObject LightObject;

    /// <summary>
    /// The bone controlling the radius of the exhaust cone.
    /// </summary>
    public GameObject ThrustScaleObject;

    public float ThrottlePercent
    {
        get { return throttleCurrentPercent * 100; }
    }

    /// <summary>
    /// Audio parameters of the engine
    /// </summary>
    public RTPC EngineStartupRTPC;
    public RTPC EngineStatusRTPC;
    public RTPC CameraDistanceRPC;

    /// <summary>
    /// Startup / stop audio handling
    /// </summary>
    AudioEngine audioEngine;

    void OnEnable()
    {
        Plane.RegisterPowerProvider(this);
        if (Plane.initialThrottleNotch)
            engineStartupPercent = 1;

        Plane.RegisterThruster(this);

        audioEngine = GetComponent<AudioEngine>();
    }

    private void OnDisable()
    {
        Plane.UnRegisterThruster(this);
    }

    void FixedUpdate()
    {
        // Update the start level of the engine (from 0 to 1). It require some power to start
        engineStartupPercent = Mathf.Clamp01(engineStartupPercent + (Plane.ThrottleNotch && Plane.GetCurrentPower() > 55 ? Time.fixedDeltaTime / engineStartupDuration : -Time.fixedDeltaTime / engineShutdownDuration));

        // Send startup informations to the engine audio
        EngineStartupRTPC.SetValue(gameObject, engineStartupPercent * 100);
        audioEngine.SetStartPercent(engineStartupPercent);

        // Force the input too zero while the engine is not fully started
        float targetInput = engineStartupPercent < 1 ? 0 : throttleDesiredPercent;
        throttleCurrentPercent = Mathf.Clamp01(throttleCurrentPercent + Mathf.Clamp(targetInput - throttleCurrentPercent, -thrustPercentAcceleration, thrustPercentAcceleration) * Time.fixedDeltaTime);

        // even at iddle, the engine provide a little thrust
        float totalInputPercent = throttleCurrentPercent + engineStartupPercent * idleEngineThrustPercent;
        Physics.AddForceAtPosition(-transform.forward * totalInputPercent * ThrustForceCurve.Evaluate(Plane.GetSpeedInNautics()), transform.position);

        // Update started engine audio value
        EngineStatusRTPC.SetValue(gameObject, totalInputPercent * 100);
        if (Camera.main)
            CameraDistanceRPC.SetValue(gameObject, Vector3.Distance(Camera.main.transform.position, transform.position) / 4);

        /// Update the visual of the thruster
        if (ThrustScaleObject)
        {
            float ExhaustScale = 1 - Mathf.Clamp(totalInputPercent * 2, 0, 0.7f) + Mathf.Clamp((totalInputPercent - 0.8f) * 4, 0, 0.7f);
            ThrustScaleObject.transform.localScale = new Vector3(ExhaustScale, ExhaustScale, ExhaustScale);
            if (VFXObject)
                VFXObject.transform.localScale = ThrustScaleObject.transform.localScale;
        }
        if (VFXObject)
        {
            VisualEffect vfx = VFXObject.GetComponent<VisualEffect>();
            vfx.SetFloat("Thrust", engineStartupPercent);
            vfx.SetFloat("AfterBurner", (totalInputPercent - 0.8f) * 10);
        }
        if (LightObject)
        {
            Light light = LightObject.GetComponent<Light>();
            light.intensity = (float)(6000.0 * System.Math.Clamp((totalInputPercent - 0.7) * 8.0, 0.0, 1.0));
        }
    }

    private void OnGUI()
    {
        // Debug
        if (!Plane.EnableDebug)
            return;
        GUILayout.BeginArea(new Rect(400, 0, 300, 200));
        GUILayout.Label("ENGINE Enabled : " + Plane.ThrottleNotch);
        GUILayout.Label("ENGINE IN : " + Plane.GetCurrentPower() + " / 45");
        GUILayout.Label("ENGINE OUT : " + engineStartupPercent * 200.0f + " (" + (engineStartupPercent * 100) + " %)");
        GUILayout.Label("ENGINE THROTTLE : desired=" + throttleDesiredPercent + " real=" + throttleCurrentPercent);
        GUILayout.Label("ENGINE THRUST ACCELERATION : " + (throttleCurrentPercent + engineStartupPercent * idleEngineThrustPercent * ThrustForceCurve.Evaluate(transform.InverseTransformDirection(Physics.velocity).z)));
        GUILayout.EndArea();
    }

    /// <summary>
    /// Set the desired thrust input value
    /// </summary>
    /// <param name="thrust_value"></param>
    public void setThrustInput(float thrust_value)
    {
        throttleDesiredPercent = Mathf.Clamp(thrust_value, 0, 1);
    }

    public float GetPower()
    {
        // The thruster also provide power to the plane, and replace the APU when started
        return engineStartupPercent * 120.0f;
    }
}

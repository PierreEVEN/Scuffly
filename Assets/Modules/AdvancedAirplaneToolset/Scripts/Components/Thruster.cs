using AK.Wwise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/**
 *  @Author : Pierre EVEN
 *  
 *  Propulseur physique de l'avion.
 *  Ne peut pas etre demarre sans APU ou source d'allimentation exterieure.
 */
[RequireComponent(typeof(AudioEngine))]
public class Thruster : PlaneComponent, IPowerProvider
{
    // Courbe du pourcentage d'acceleration du moteur en fonction de l'input
    public AnimationCurve ThrustPercentCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.7f, 0.6f), new Keyframe(1, 1.1f) });

    // Courbe de force de poussee maximale en fonction de la vitesse
    public AnimationCurve ThrustForceCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 3000000.0f), new Keyframe(300, 8000000.0f) });

    // Duree de mise en route et d'extinction du reacteur
    public float engineStartupDuration = 39;
    public float engineShutdownDuration = 39;
    [HideInInspector]
    public float engineStartupPercent = 0; // pourcentage de rotation de la turbine - par simplification, vaut 1 une fois le moteur en marche
    public float idleEngineThrustPercent = 0.001f;

    public float thrustPercentAcceleration = 0.4f; // vitesse d'acceleration du moteur
    private float throttleDesiredPercent = 0; // pourcentage de puissance du moteur desire
    private float throttleCurrentPercent = 0; // pourcentage de puissance reel du moteur

    public GameObject VFXObject;
    public GameObject LightObject;
    public GameObject ThrustScaleObject;

    public float ThrottlePercent
    {
        get { return throttleCurrentPercent * 100; }
    }

    // Parametres du son du moteur
    public RTPC EngineStartupRTPC;
    public RTPC EngineStatusRTPC;
    public RTPC CameraDistanceRPC;

    // Son du moteur
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
        // Met a jour la rotation du moteur en fonction de son etat d'activation et de la puissance d'allimentation de l'avion
        engineStartupPercent = Mathf.Clamp01(engineStartupPercent + (Plane.ThrottleNotch && Plane.GetCurrentPower() > 55 ? Time.fixedDeltaTime / engineStartupDuration : -Time.fixedDeltaTime / engineShutdownDuration));

        EngineStartupRTPC.SetValue(gameObject, engineStartupPercent * 100);
        audioEngine.SetStartPercent(engineStartupPercent);

        // Tant que le moteur n'est pas demarre, on ne fait rien
        float targetInput = engineStartupPercent < 1 ? 0 : throttleDesiredPercent;

        throttleCurrentPercent = Mathf.Clamp01(throttleCurrentPercent + Mathf.Clamp(targetInput - throttleCurrentPercent, -thrustPercentAcceleration, thrustPercentAcceleration) * Time.fixedDeltaTime);
        // Meme a l'arret, le moteur produit une legere poussee
        float totalInputPercent = throttleCurrentPercent + engineStartupPercent * idleEngineThrustPercent;
        Physics.AddForceAtPosition(-transform.forward * totalInputPercent * ThrustForceCurve.Evaluate(Plane.GetSpeedInNautics()), transform.position);

        EngineStatusRTPC.SetValue(gameObject, totalInputPercent * 100);
        if (Camera.main)
            CameraDistanceRPC.SetValue(gameObject, Vector3.Distance(Camera.main.transform.position, transform.position) / 4);

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

    public void setThrustInput(float thrust_value)
    {
        throttleDesiredPercent = Mathf.Clamp(thrust_value, 0, 1);
    }

    public float GetPower()
    {
        // produit 200 Kva a pleine puissance une fois demarre
        return engineStartupPercent * 120.0f;
    }
}

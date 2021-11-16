using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *  @Author : Pierre EVEN
 *  
 *  Propulseur physique de l'avion.
 *  Ne peut pas etre demarre sans APU ou source d'allimentation exterieure.
 */
public class Thruster : PlaneComponent, IPowerProvider
{
    // Courbe du pourcentage d'acceleration du moteur en fonction de l'input
    public AnimationCurve ThrustPercentCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.7f, 0.6f), new Keyframe(1, 1.1f) });

    // Courbe de force de poussee maximale en fonction de la vitesse
    public AnimationCurve ThrustForceCurve = new AnimationCurve(new Keyframe[]{new Keyframe(0, 3000000.0f), new Keyframe(300, 8000000.0f)});

    public float engineStartupPercentPerSeconds = 0.1f;
    private float engineStartupPercent = 0; // pourcentage de rotation de la turbine - par simplification, vaut 1 une fois le moteur en marche
    public float idleEngineThrustPercent = 0.001f;

    public float thrustPercentAcceleration = 0.4f; // vitesse d'acceleration du moteur
    private float throttleDesiredPercent = 0; // pourcentage de puissance du moteur desire
    private float throttleCurrentPercent = 0; // pourcentage de puissance reel du moteur

    void OnEnable()
    {
        Plane.powerProviders.Add(this);
        if (Plane.initialThrottleNotch)
            engineStartupPercent = 1;
    }

    private void OnDisable()
    {
        Plane.powerProviders.Remove(this);
    }

    void Update()
    {
        // Met a jour la rotation du moteur en fonction de son etat d'activation et de la puissance d'allimentation de l'avion
        engineStartupPercent = Mathf.Clamp01(engineStartupPercent + (Plane.ThrottleNotch && Plane.GetCurrentPower() > 55 ? Time.deltaTime * engineStartupPercentPerSeconds : -Time.deltaTime * engineStartupPercentPerSeconds));

        // Tant que le moteur n'est pas demarre, on ne fait rien
        float targetInput = engineStartupPercent < 1 ? 0 : throttleDesiredPercent;

        throttleCurrentPercent = Mathf.Clamp01(throttleCurrentPercent + Mathf.Clamp(targetInput - throttleCurrentPercent, -thrustPercentAcceleration, thrustPercentAcceleration) * Time.deltaTime);
        // Meme a l'arret, le moteur produit une legere poussee
        float totalInputPercent = throttleCurrentPercent + engineStartupPercent * idleEngineThrustPercent;
        float forwardVelocity = transform.InverseTransformDirection(Physics.velocity).z;
        Physics.AddForceAtPosition(transform.forward * Time.deltaTime * totalInputPercent * ThrustForceCurve.Evaluate(forwardVelocity), transform.position);
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

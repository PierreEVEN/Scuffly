using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *  @Author : Pierre EVEN
 *  
 *  Propulseur physique de l'avion.
 *  Ne peut pas etre demarre sans APU ou source d'allimentation exterieure.
 */
public class Thruster : MonoBehaviour
{
    Rigidbody PhysicBody;
    private APU PlaneAPU;

    // ENGINE
    public float EngineAcceleration = 0.1f; // what is the acceleration rate of the engine
    public float StartupEngineAcceleration = 0.004f; // what is the acceleration rate of the engine

    // Courbe de force de poussee maximale en fonction de la vitesse
    public AnimationCurve ThrustForceCurve = new AnimationCurve(new Keyframe[]{new Keyframe(0, 1500000.0f), new Keyframe(300, 400000.0f)});

    private float FinalEngineInput = 0.0f; // Real thrust power of the engine

    // INPUT
    private float EngineInput = 0.0f; // The engine input value given by the pilot (0 - 1)

    // PHYSIC
    private Vector3 thrustVector = new Vector3(); // Real thrust vector of the engine (currentThrust * vectorial thrust)

    void OnGUI()
    {
        // @todo : remove debug
        GUILayout.Space(100);
        GUILayout.TextArea("Thrust input : " + EngineInput + " / engine status : " + FinalEngineInput);
        GUILayout.TextArea("Thrust force : " + thrustVector.magnitude);
    }

    void Start()
    {
        PhysicBody = gameObject.GetComponentInParent<Rigidbody>();
        if (!PhysicBody)
            Debug.LogError("Thruster is not attached to an object containing a physic body");

        PlaneAPU = gameObject.GetComponentInParent<APU>();
        if (!PlaneAPU)
            Debug.LogError("thruster need APU to start");
    }

    void Update()
    {
        if (!PhysicBody)
            return;

        bool enoughPower = FinalEngineInput > 0.01f || PlaneAPU.IsReady();

        float EngineDesiredInput = enoughPower ? EngineInput : 0.0f;

        if (FinalEngineInput < EngineDesiredInput)
            FinalEngineInput += Mathf.Min(EngineDesiredInput - FinalEngineInput, Time.deltaTime * (FinalEngineInput < 0.02f ? StartupEngineAcceleration : EngineAcceleration));
        else
            FinalEngineInput -= Mathf.Min(FinalEngineInput - EngineDesiredInput, Time.deltaTime * (FinalEngineInput < 0.02f ? StartupEngineAcceleration : EngineAcceleration));

        // Compute thrust vector
        thrustVector = gameObject.transform.forward.normalized * FinalEngineInput * ThrustForceCurve.Evaluate(200);

        PhysicBody.AddForceAtPosition(thrustVector * Time.deltaTime, gameObject.transform.position);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + thrustVector * -0.006f, Color.cyan);
    }

    public void set_thrust_input(float thrust_value)
    {
        EngineInput = Mathf.Clamp(thrust_value, 0, 1);
    }
}

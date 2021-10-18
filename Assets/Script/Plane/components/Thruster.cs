using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *  @Author : Pierre EVEN
 */

public class Thruster : MonoBehaviour
{
    Rigidbody PhysicBody;
    private APU PlaneAPU;

    // ENGINE
    public float MaxThrustPower = 3000000.0f; // max thrust power of the engine
    public float EngineAcceleration = 0.1f; // what is the acceleration rate of the engine
    public float StartupEngineAcceleration = 0.004f; // what is the acceleration rate of the engine
    private float FinalEngineInput = 0.0f; // Real thrust power of the engine

    // INPUT
    private float EngineInput = 0.0f; // The engine input value given by the pilot (0 - 1)

    // PHYSIC
    private Vector3 thrustVector = new Vector3(); // Real thrust vector of the engine (currentThrust * vectorial thrust)

    void OnGUI()
    {
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

        bool enoughPower = FinalEngineInput > 0.09f || PlaneAPU.IsReady();

        float EngineDesiredInput = enoughPower ? EngineInput : 0.0f;

        if (FinalEngineInput < EngineDesiredInput)
            FinalEngineInput += Mathf.Min(EngineDesiredInput - FinalEngineInput, Time.deltaTime * (FinalEngineInput < 0.02f ? StartupEngineAcceleration : EngineAcceleration));
        else
            FinalEngineInput -= Mathf.Min(FinalEngineInput - EngineDesiredInput, Time.deltaTime * (FinalEngineInput < 0.02f ? StartupEngineAcceleration : EngineAcceleration));

        // Compute thrust vector
        thrustVector = gameObject.transform.forward.normalized * FinalEngineInput * MaxThrustPower;

        PhysicBody.AddForceAtPosition(thrustVector * Time.deltaTime, gameObject.transform.position);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + thrustVector * -0.006f, Color.cyan);
    }

    public void set_thrust_input(float thrust_value)
    {
        EngineInput = Mathf.Clamp(thrust_value, 0, 1);
    }
}

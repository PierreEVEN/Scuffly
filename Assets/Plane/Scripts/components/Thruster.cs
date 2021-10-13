using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    // PMANE PHYSIC BODY
    Rigidbody targetBody;

    // ENGINE
    public float maxThrust = 200.0f; // max thrust power of the engine
    public float thrustAcceleration = 20.0f; // what is the acceleration rate of the engine
    public float engineInputLatency = 1.0f; // What is the latency of the engine's input

    private float engineLatentInput = 0.0f; // The simulated input of the engine including latency
    private float enginePilotInput = 0.0f; // The engine input value given by the pilot
    private float currentThrust = 0.0f; // Real thrust power of the engine
    private Vector3 thrustVector = new Vector3(); // Real thrust vector of the engine (currentThrust * vectorial thrust)

    void Start()
    {
        targetBody = gameObject.GetComponentInParent<Rigidbody>();
        if (!targetBody)
            Debug.LogError("Thruster is not attached to an object containing a physic body");

    }

    void Update()
    {
        if (!targetBody)
            return;
        UpdateThrustVector();
        targetBody.AddForceAtPosition(thrustVector, gameObject.transform.position);
    }


    private void UpdateThrustVector()
    {
        // Compute thrust power (The input latency of the engine is simulated with a linear interpolation between the real input and the current engine input)
        engineLatentInput = Mathf.Lerp(engineLatentInput, enginePilotInput, engineInputLatency * Time.deltaTime);
        currentThrust = Mathf.Lerp(currentThrust, engineLatentInput * maxThrust, thrustAcceleration * Time.deltaTime);

        // Compute thrust vector
        thrustVector = targetBody.transform.forward.normalized;
        thrustVector *= currentThrust;
    }
    public void set_thrust_input(float thrust_value)
    {
        enginePilotInput = thrust_value;
    }
}

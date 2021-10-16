using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    // PMANE PHYSIC BODY
    Rigidbody targetBody;

    // ENGINE
    public float maxThrust = 20000.0f; // max thrust power of the engine
    public float thrustAcceleration = 200f; // what is the acceleration rate of the engine
    public float engineInputLatency = 100.0f; // What is the latency of the engine's input

    private float engineLatentInput = 0.0f; // The simulated input of the engine including latency
    private float enginePilotInput = 0.0f; // The engine input value given by the pilot
    private float currentThrust = 0.0f; // Real thrust power of the engine
    private Vector3 thrustVector = new Vector3(); // Real thrust vector of the engine (currentThrust * vectorial thrust)

    void OnGUI()
    {
        GUILayout.TextArea("Thrust input : " + enginePilotInput);
        GUILayout.TextArea("Thrust force : " + thrustVector.magnitude);
    }

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
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + thrustVector * -0.006f, Color.cyan);
    }


    private void UpdateThrustVector()
    {
        // Compute thrust power (The input latency of the engine is simulated with a linear interpolation between the real input and the current engine input)
        engineLatentInput = Mathf.Lerp(engineLatentInput, Mathf.Clamp(enginePilotInput, 0, 1), engineInputLatency * Time.deltaTime);
        currentThrust = Mathf.Lerp(currentThrust, engineLatentInput * maxThrust, thrustAcceleration * Time.deltaTime);

        // Compute thrust vector
        thrustVector = gameObject.transform.right.normalized;
        thrustVector *= currentThrust * -1;
    }
    public void set_thrust_input(float thrust_value)
    {
        enginePilotInput = Mathf.Clamp(thrust_value, 0, 1);
    }
}


using UnityEngine;
using UnityEngine.InputSystem;
/**
*  @Author : Pierre EVEN
*/

[RequireComponent(typeof(PlayerManager))]
public class PlaneInputInterface : MonoBehaviour
{
    private PlayerManager playerManager;

    private float thrustValue = 0;
    private float upValue = 0;
    private float rightValue = 0;
    private float rollValue = 0;

    private float thrustInput = 0;
    private float upInput = 0;
    private float rightInput = 0;
    private float rollInput = 0;

    private bool enableEngine = false;
    private bool apuEnabled = false;
    void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    void OnGUI()
    {
        if (!playerManager.controlledPlane.Value)
            return;
        GUILayout.Space(50);
        GUILayout.TextArea("Velocity : " + playerManager.controlledPlane.Value.GetComponent<Rigidbody>().velocity.magnitude + " m/s  |  " + playerManager.controlledPlane.Value.GetComponent<Rigidbody>().velocity.magnitude * 3.6 + " km/h  |  " + playerManager.controlledPlane.Value.GetComponent<Rigidbody>().velocity.magnitude * 1.94384519992989f + " noeuds");
    }

    public Vector3 MassCenter = new Vector3(0, 0, 0);

    void Update()
    {
        thrustValue = Mathf.Clamp(thrustInput, 0, 1);
        upValue = Mathf.Clamp(upInput, -1, 1);
        rightValue = Mathf.Clamp(rightInput, -1, 1);
        rollValue = Mathf.Clamp(rollInput, -1, 1);

        if (!playerManager.controlledPlane.Value)
            return;


        SetThrustInput(enableEngine ? thrustValue * 0.9f + 0.1f : 0);
        setPitchInput(upValue);
        setYawInput(rightValue);
        setRollInput(rollValue);

    }

    public void OnThrustAxis(InputValue input)
    {
        thrustInput = input.Get<float>() * 0.5f + 0.5f;
    }

    public void OnYawAxis(InputValue input)
    {
        rightInput = input.Get<float>();
    }

    public void OnRollAxis(InputValue input)
    {
        rollInput = input.Get<float>();
    }
    public void OnPitchAxis(InputValue input)
    {
        upInput = input.Get<float>();
    }
    public void OnThrust(InputValue input)
    {
        thrustInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }
    public void OnUp(InputValue input)
    {
        upInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }
    public void OnRight(InputValue input)
    {
        rightInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }
    public void OnRoll(InputValue input)
    {
        rollInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }
    public void OnSwitchAPU()
    {
        SetApuEnabled(!apuEnabled);
    }

    public void SetApuEnabled(bool enable)
    {
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<APU>())
            if (enable)
                part.StartApu();
            else
                part.StopApu();
        apuEnabled = !enable;
    }

    public void OnSwitchEngine()
    {
        SetEngineEnabled(!enableEngine);
    }

    public void SetEngineEnabled(bool enabled)
    {
        enableEngine = enabled;
    }

    public void OnSwitchGear()
    {
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<PlaneWheelController>())
            part.Switch();
    }
    public void onSwitchBattery() { }

    public void OnSetBrake(InputValue input)
    {
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<WheelCollider>())
                part.brakeTorque = Mathf.Clamp(input.Get<float>(), 0, 1) * 3000;
    }

    public void SetThrustInput(float value)
    {
        value = Mathf.Clamp(value, 0, 1);

        if (!playerManager.controlledPlane.Value)
            return;
        foreach (var thruster in playerManager.controlledPlane.Value.GetComponentsInChildren<Thruster>())
        {
            thruster.set_thrust_input(value);
        }
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<WheelCollider>())
            part.motorTorque = 0.01f;
    }

    public void setPitchInput(float value)
    {
        value = Mathf.Clamp(value, -1, 1);

        if (!playerManager.controlledPlane.Value)
            return;

        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Pitch")
                part.setInput(value * -1);
    }

    private void setYawInput(float value)
    {
        value = Mathf.Clamp(value, -1, 1);

        if (!playerManager.controlledPlane.Value)
            return;
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<WheelCollider>())
            if (part.tag == "Yaw")
                part.steerAngle = Mathf.Pow(value, 3) * 65;

        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Yaw")
                part.setInput(value);
    }
    public void setRollInput(float value)
    {
        value = Mathf.Clamp(value, -1, 1);

        if (!playerManager.controlledPlane.Value)
            return;
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Roll")
                part.setInput(value);
    }
}

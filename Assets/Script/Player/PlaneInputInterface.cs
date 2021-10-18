
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
    private bool ExtentGear = false;
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
        thrustValue = Mathf.Clamp(thrustValue + thrustInput * Time.deltaTime * 10, 0, 1);
        upValue = Mathf.Clamp(upValue + upInput * Time.deltaTime, -1, 1);
        rightValue = Mathf.Clamp(rightValue + rightInput * Time.deltaTime, -1, 1);
        rollValue = Mathf.Lerp(rollValue, rollInput, Time.deltaTime * 2);

        if (!playerManager.controlledPlane.Value)
            return;


        SetThrustInput(enableEngine ? thrustValue * 0.9f + 0.1f : 0);
        setPitchInput(upValue);
        setYawInput(rightValue);
        setRollInput(rollValue);

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
        if (!playerManager.controlledPlane.Value)
            return;
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<APU>())
            if (part.IsReady())
                part.StopApu();
            else
                part.StartApu();
    }

    public void OnSwitchEngine()
    {
        enableEngine = !enableEngine;
    }
    public void OnSwitchGear()
    {
        ExtentGear = !ExtentGear;
    }
    public void onSwitchBattery() { }


    private void SetThrustInput(float value)
    {
        if (!playerManager.controlledPlane.Value)
            return;
        foreach (var thruster in playerManager.controlledPlane.Value.GetComponentsInChildren<Thruster>())
        {
            thruster.set_thrust_input(value);
        }
    }

    private void setPitchInput(float value)
    {
        if (!playerManager.controlledPlane.Value)
            return;
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Pitch")
                part.setInput(value * -1);
    }

    private void setYawInput(float value)
    {
        if (!playerManager.controlledPlane.Value)
            return;
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Yaw")
                part.setInput(value);
    }
    private void setRollInput(float value)
    {
        if (!playerManager.controlledPlane.Value)
            return;
        foreach (var part in playerManager.controlledPlane.Value.GetComponentsInChildren<MobilePart>())
            if (part.tag == "Roll")
                part.setInput(value);
    }
}

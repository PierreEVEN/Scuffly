
using UnityEngine;
using UnityEngine.InputSystem;
/**
*  @Author : Pierre EVEN
*/

[RequireComponent(typeof(PlayerManager))]
public class PlanePlayerInputs : MonoBehaviour
{
    private PlayerManager playerManager;

    void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    float LastVelocity = 0;
    float Acceleration = 0;
    float pitchInput = 0;
    float pitchTrim = 0;
    void OnGUI()
    {
        if (!playerManager.controlledPlane)
            return;
        GUILayout.Space(250);
        GUILayout.TextArea("Velocity : " + LastVelocity + " m/s  |  " + LastVelocity * 3.6 + " km/h  |  " + LastVelocity * 1.94384519992989f + " noeuds");
        GUILayout.TextArea("Force : " + Acceleration + " m/s  |  " + Acceleration / 9.81 + " g");
        GUILayout.TextArea("Attitude : " + playerManager.controlledPlane.GetAttitude());
        GUILayout.TextArea("Roll : " + playerManager.controlledPlane.GetRoll());
        GUILayout.TextArea("Heading : " + playerManager.controlledPlane.GetHeading());
    }

    public Vector3 MassCenter = new Vector3(0, 0, 0);

    void Update()
    {

        if (!playerManager.controlledPlane)
            return;

        pitchTrim += trimIncreaseInput * Time.deltaTime * 0.5f;
        pitchTrim = Mathf.Clamp(pitchTrim, -1, 1);

        playerManager.controlledPlane.SetPitchInput(Mathf.Clamp(pitchInput + pitchTrim, -1, 1));

    }

    public void FixedUpdate()
    {

        float currentVelocity = playerManager.controlledPlane.GetComponent<Rigidbody>().velocity.magnitude;
        Acceleration = (Mathf.Abs(currentVelocity - LastVelocity)) / Time.fixedDeltaTime + 9.81f;
        LastVelocity = currentVelocity;
    }

    /**
     * Direct axis
     */

    public void OnAxisThrottle(InputValue input)
    {
        if (!playerManager.controlledPlane)
            return;

        playerManager.controlledPlane.SetThrustInput(input.Get<float>() * 0.5f + 0.5f);
    }

    public void OnAxisYaw(InputValue input)
    {
        if (!playerManager.controlledPlane)
            return;

        playerManager.controlledPlane.SetYawInput(input.Get<float>());
    }

    public void OnAxisRoll(InputValue input)
    {
        if (!playerManager.controlledPlane)
            return;

        playerManager.controlledPlane.SetRollInput(input.Get<float>());
    }
    public void OnAxisPitch(InputValue input)
    {
        if (!playerManager.controlledPlane)
            return;

        pitchInput = input.Get<float>();
    }

    /**
     * Button and keyboard axis
     */

    float currentKeyboardThrottle = 0;
    float trimIncreaseInput = 0;
    float currentKeyboardYaw = 0;
    float currentKeyboardRoll = 0;

    public void OnIncreaseThrottle(InputValue input)
    {
        currentKeyboardThrottle = Mathf.Clamp(currentKeyboardThrottle + input.Get<float>(), 0, 1);
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.SetThrustInput(currentKeyboardThrottle);
    }

    public void OnSetPitch(InputValue input)
    {
        trimIncreaseInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }
    public void OnSetYaw(InputValue input)
    {
        currentKeyboardYaw = Mathf.Clamp(input.Get<float>(), -1, 1);
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.SetYawInput(currentKeyboardYaw);
    }
    public void OnSetRoll(InputValue input)
    {
        currentKeyboardRoll = Mathf.Clamp(input.Get<float>(), -1, 1);
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.SetRollInput(currentKeyboardRoll);
    }

    public void OnSwitchAPU()
    {
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.ApuSwitch = !playerManager.controlledPlane.ApuSwitch;
    }

    public void OnSwitchThrottleNotch()
    {
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.ThrottleNotch = !playerManager.controlledPlane.ThrottleNotch;
        playerManager.controlledPlane.SetThrustInput(currentKeyboardThrottle);
    }

    public void OnSwitchGear()
    {
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.RetractGear = !playerManager.controlledPlane.RetractGear;
    }

    public void OnSwitchBrakes()
    {
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.Brakes = !playerManager.controlledPlane.Brakes;
    }

    public void OnSetBrake(InputValue input)
    {
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.Brakes = input.isPressed;
    }

    public void OnShoot()
    {
        if (!playerManager.controlledPlane)
            return;
        playerManager.controlledPlane.Shoot();
    }

}

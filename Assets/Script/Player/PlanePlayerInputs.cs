
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Interface between the player keyboard and the current controlled plane
/// </summary>
[RequireComponent(typeof(PlayerManager))]
public class PlanePlayerInputs : MonoBehaviour
{
    private PlayerManager playerManager;

    void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    /// <summary>
    /// The pitch input from keyboard act like a trim
    /// </summary>
    float pitchInput = 0;
    float pitchTrim = 0;

    /// <summary>
    /// Current keyboard input state
    /// </summary>
    float currentKeyboardThrottle = 0;
    float trimIncreaseInput = 0;
    float currentKeyboardYaw = 0;
    float currentKeyboardRoll = 0;

    public Vector3 MassCenter = new Vector3(0, 0, 0);

    bool enableInputs = false;

    /// <summary>
    /// Are input enabled
    /// </summary>
    public bool EnableInputs
    {
        set
        {
            enableInputs = value;
        }
        get
        {
            return enableInputs && playerManager && playerManager.controlledPlane && !playerManager.disableInputs;
        }
    }


    void Update()
    {
        if (!EnableInputs)
            return;

        // Update the pitch of the plane
        pitchTrim += trimIncreaseInput * Time.deltaTime * 1.5f;
        pitchTrim = Mathf.Clamp(pitchTrim, -1, 1);
        playerManager.controlledPlane.SetPitchInput(Mathf.Clamp(pitchInput + pitchTrim, -1, 1));
    }

    /**
     * Direct axis : joystick / analogic controls
     */

    public void OnAxisThrottle(InputValue input)
    {
        if (!EnableInputs)
            return;

        playerManager.controlledPlane.SetThrustInput(input.Get<float>() * 0.5f + 0.5f);
    }

    public void OnAxisYaw(InputValue input)
    {
        if (!EnableInputs)
            return;

        playerManager.controlledPlane.SetYawInput(input.Get<float>());
    }

    public void OnAxisRoll(InputValue input)
    {
        if (!EnableInputs)
            return;

        playerManager.controlledPlane.SetRollInput(input.Get<float>());
    }
    public void OnAxisPitch(InputValue input)
    {
        if (!EnableInputs)
            return;

        pitchInput = input.Get<float>();
    }

    /**
     * Button and keyboard axis
     */

    public void OnIncreaseThrottle(InputValue input)
    {
        currentKeyboardThrottle = Mathf.Clamp(currentKeyboardThrottle + input.Get<float>(), 0, 1);
        if (!EnableInputs)
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
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.SetYawInput(currentKeyboardYaw);
    }
    public void OnSetRoll(InputValue input)
    {
        currentKeyboardRoll = Mathf.Clamp(input.Get<float>(), -1, 1);
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.SetRollInput(currentKeyboardRoll);
    }

    public void OnSwitchAPU()
    {
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.EnableAPU = !playerManager.controlledPlane.EnableAPU;
    }

    public void OnSwitchPower()
    {
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.MainPower = !playerManager.controlledPlane.MainPower;
    }

    public void OnSwitchThrottleNotch()
    {
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.ThrottleNotch = !playerManager.controlledPlane.ThrottleNotch;
        playerManager.controlledPlane.SetThrustInput(currentKeyboardThrottle);
    }

    public void OnSwitchGear()
    {
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.RetractGear = !playerManager.controlledPlane.RetractGear;
    }

    public void OnSwitchBrakes()
    {
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.ParkingBrakes = !playerManager.controlledPlane.ParkingBrakes;
    }

    public void OnSetBrake(InputValue input)
    {
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.ParkingBrakes = input.isPressed;
    }

    public void OnShoot()
    {
        if (!EnableInputs)
            return;

        WeaponManager weaponManager = playerManager.controlledPlane.GetComponent<WeaponManager>();
        if (!weaponManager)
            return;

        weaponManager.BeginShoot();
    }
    public void OnEndShoot()
    {
        WeaponManager weaponManager = playerManager.controlledPlane.GetComponent<WeaponManager>();
        if (!weaponManager)
            return;

        weaponManager.EndShoot();
    }

    public void OnSwitchCanon()
    {
        if (!EnableInputs)
            return;

        WeaponManager weaponManager = playerManager.controlledPlane.GetComponent<WeaponManager>();
        if (!weaponManager)
            return;

        weaponManager.SwitchToCanon();
    }
    public void OnSwitchAirGround()
    {
        if (!EnableInputs)
            return;

        WeaponManager weaponManager = playerManager.controlledPlane.GetComponent<WeaponManager>();
        if (!weaponManager)
            return;

        weaponManager.AirGroundMode();
    }

    public void OnSwitchAirAir()
    {
        if (!EnableInputs)
            return;

        WeaponManager weaponManager = playerManager.controlledPlane.GetComponent<WeaponManager>();
        if (!weaponManager)
            return;

        weaponManager.AirAirMode();
    }

    public void OnEnableWeapons()
    {
        if (!EnableInputs)
            return;

        WeaponManager weaponManager = playerManager.controlledPlane.GetComponent<WeaponManager>();
        if (!weaponManager)
            return;

        weaponManager.IsToggledOn = !weaponManager.IsToggledOn;
    }

    public void OnSwitchCanopy()
    {
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.OpenCanopy = !playerManager.controlledPlane.OpenCanopy;
    }

    public void OnBrakes(InputValue input)
    {
        if (!EnableInputs)
            return;
        playerManager.controlledPlane.Brakes = input.Get<float>() > 0.5f;
    }
}
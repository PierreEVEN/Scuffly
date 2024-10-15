
using UnityEngine;

public enum ESwitchTarget
{
    None,
    MainPower,
    APU,
    Gear,
    Brakes,
    ThrottleNotch,
    Weapons,
    LandingLights
}

/// <summary>
/// A simple switch with 2 positions ON/OFF
/// </summary>
[ExecuteInEditMode]
public class FlipFlopSwitch : SwitchBase
{
    /// <summary>
    /// Current switch status
    /// </summary>
    public bool On = false;

    /// <summary>
    /// The action of the switch
    /// </summary>
    public ESwitchTarget modifiedProperty;
    
    /// <summary>
    /// Position and rotation in On state
    /// </summary>
    public Vector3 PositionOn = new Vector3();
    public Quaternion RotationOn = new Quaternion();

    /// <summary>
    /// Position and rotation in Off state
    /// </summary>
    public Vector3 PositionOff = new Vector3();
    public Quaternion RotationOff = new Quaternion();
    
    /// <summary>
    /// Switch audio effect
    /// </summary>
    public AK.Wwise.Event PlayEvent;

    private void Update()
    {
        //Make the button position match the real value
        if (Application.isPlaying && modifiedProperty != ESwitchTarget.None)
        {
            bool newOn = false;
            switch (modifiedProperty)
            {
                case ESwitchTarget.MainPower:
                    newOn = Plane.MainPower;
                    break;
                case ESwitchTarget.APU:
                    newOn = Plane.EnableAPU;
                    break;
                case ESwitchTarget.Gear:
                    newOn = Plane.RetractGear;
                    break;
                case ESwitchTarget.Brakes:
                    newOn = Plane.ParkingBrakes;
                    break;
                case ESwitchTarget.ThrottleNotch:
                    newOn = Plane.ThrottleNotch;
                    break;
                case ESwitchTarget.Weapons:
                    newOn = WeaponSystem.IsToggledOn;
                    break;
                case ESwitchTarget.None:
                    break;
                case ESwitchTarget.LandingLights:
                    newOn = Plane.LandingLights;
                    break;
            }
            // Updathe the button position
            if (newOn != On)
            {
                On = newOn;
                PlayEvent.Post(gameObject);
            }
        }

        // Set the button transform
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            child.localPosition = On ? PositionOn : PositionOff;
            child.localRotation = On ? RotationOn : RotationOff;
        }
    }

    public override void Switch()
    {
        // Event called on press
        On = !On;
        PlayEvent.Post(gameObject);
        if (modifiedProperty == ESwitchTarget.None) // unused button
            return;

        // Modify the property bound to the button
        switch (modifiedProperty)
        {
            case ESwitchTarget.MainPower:
                Plane.MainPower = On;
                break;
            case ESwitchTarget.APU:
                Plane.EnableAPU = On;
                break;
            case ESwitchTarget.Gear:
                Plane.RetractGear = On;
                break;
            case ESwitchTarget.Brakes:
                Plane.ParkingBrakes = On;
                break;
            case ESwitchTarget.Weapons:
                WeaponSystem.IsToggledOn = On;
                break;
            case ESwitchTarget.ThrottleNotch:
                Plane.ThrottleNotch = On;
                break;
            case ESwitchTarget.LandingLights:
                Plane.LandingLights = On;
                break;
        }
    }

    public override void Release() {}
}

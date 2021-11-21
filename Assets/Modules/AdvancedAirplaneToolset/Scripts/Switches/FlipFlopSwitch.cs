
using UnityEngine;

public enum ESwitchTarget
{
    None,
    MainPower,
    APU,
    Gear,
    Brakes,
    ThrottleNotch
}

[ExecuteInEditMode]
public class FlipFlopSwitch : SwitchBase
{
    public bool On = false;
    public ESwitchTarget modifiedProperty;

    public Vector3 PositionOn = new Vector3();
    public Quaternion RotationOn = new Quaternion();

    public Vector3 PositionOff = new Vector3();
    public Quaternion RotationOff = new Quaternion();

    public AK.Wwise.Event PlayEvent;

    private void Update()
    {
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
                    newOn = Plane.Brakes;
                    break;
                case ESwitchTarget.ThrottleNotch:
                    newOn = Plane.ThrottleNotch;
                    break;
            }
            if (newOn != On)
            {
                On = newOn;
                PlayEvent.Post(gameObject);
            }
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            child.localPosition = On ? PositionOn : PositionOff;
            child.localRotation = On ? RotationOn : RotationOff;
        }
    }

    public override void Switch()
    {
        On = !On;
        PlayEvent.Post(gameObject);
        if (modifiedProperty == ESwitchTarget.None)
            return;

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
                Plane.Brakes = On;
                break;
            case ESwitchTarget.ThrottleNotch:
                Plane.ThrottleNotch = On;
                break;
        }
    }
}

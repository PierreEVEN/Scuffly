using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESwitchTarget {
    MainPower,
    APU,
    Gear,
    Brakes,
    ThrottleNotch
}

[ExecuteInEditMode]
public class ClickableSwitch : PlaneComponent
{
    public bool On = false;
    public ESwitchTarget modifiedProperty;
    public string Description;

    public Vector3 PositionOn = new Vector3();
    public Quaternion RotationOn = new Quaternion();

    public Vector3 PositionOff = new Vector3();
    public Quaternion RotationOff = new Quaternion();

    public void Switch()
    {
        Debug.Log("clicked");
        On = !On;
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

    private void Update()
    {
        if (Application.isPlaying)
        {
            switch (modifiedProperty)
            {
                case ESwitchTarget.MainPower:
                    On = Plane.MainPower;
                    break;
                case ESwitchTarget.APU:
                    On = Plane.EnableAPU;
                    break;
                case ESwitchTarget.Gear:
                    On = Plane.RetractGear;
                    break;
                case ESwitchTarget.Brakes:
                    On = Plane.Brakes;
                    break;
                case ESwitchTarget.ThrottleNotch:
                    On = Plane.ThrottleNotch;
                    break;
            }
        }

        transform.localPosition = On ? PositionOn : PositionOff;
        transform.localRotation = On ? RotationOn : RotationOff;
    }
}

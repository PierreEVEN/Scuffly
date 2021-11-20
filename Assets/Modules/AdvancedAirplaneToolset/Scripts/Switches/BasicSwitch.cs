
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
public class BasicSwitch : PlaneComponent
{
    public bool On = false;
    public ESwitchTarget modifiedProperty;
    public string Description;

    public Vector3 PositionOn = new Vector3();
    public Quaternion RotationOn = new Quaternion();

    public Vector3 PositionOff = new Vector3();
    public Quaternion RotationOff = new Quaternion();

    public AK.Wwise.Event PlayEvent;

    private void OnEnable()
    {
    }

    public void Switch()
    {
        Debug.Log("switch");
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

    private void Update()
    {
        if (Application.isPlaying && modifiedProperty != ESwitchTarget.None)
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

        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            child.localPosition = On ? PositionOn : PositionOff;
            child.localRotation = On ? RotationOn : RotationOff;
        }
    }

    public void StartOver()
    {
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.layer = 3;
    }

    public void StopOver()
    {
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.layer = 0;
    }
}

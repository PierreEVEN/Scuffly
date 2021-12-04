
using UnityEngine;

// Action de l'interrupteur
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

// Interrupteur a deux positions ON/OFF, a placer dans le cockpit de l'avion
[ExecuteInEditMode]
public class FlipFlopSwitch : SwitchBase
{
    // Position de base
    public bool On = false;
    // Action de l'interrupteur
    public ESwitchTarget modifiedProperty;

    // Positions et rotations de l'interupteur en position ON et OFF
    public Vector3 PositionOn = new Vector3();
    public Quaternion RotationOn = new Quaternion();

    public Vector3 PositionOff = new Vector3();
    public Quaternion RotationOff = new Quaternion();

    // Son a l'appuis du bouton
    public AK.Wwise.Event PlayEvent;

    private void Update()
    {
        // Detecte un eventuellement changement exterieur de la variable observee
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
            // Met a jour l'etat bouton
            if (newOn != On)
            {
                On = newOn;
                PlayEvent.Post(gameObject);
            }
        }

        // Met a jour la position graphique de l'interupteur
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            child.localPosition = On ? PositionOn : PositionOff;
            child.localRotation = On ? RotationOn : RotationOff;
        }
    }

    // Change la position du bouton : On = !On
    public override void Switch()
    {
        On = !On;
        PlayEvent.Post(gameObject);
        if (modifiedProperty == ESwitchTarget.None)
            return;

        // Applique la modification a l'avion en fonction de la propriété modifiée
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

    public override void Release()
    {
    }
}

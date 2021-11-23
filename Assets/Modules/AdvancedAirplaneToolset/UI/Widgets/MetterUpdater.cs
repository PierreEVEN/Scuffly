using UnityEngine;
using UnityEngine.UI;

public enum MetterType
{
    Altitude,
    Velocity,
    OilPressure,
    NozPosOpen,
    RPMPercent,
    Fuel,
    HydrolicPressure
}

public class MetterUpdater : PlaneComponent
{
    public GameObject metterText;

    public GameObject metterNeedle;

    public MetterType metterType = MetterType.Altitude;

    public float needleScale = 0.0001f;

    void Update()
    {
        float foots = getValue();
        metterNeedle.transform.localRotation = Quaternion.Euler(0, 0, foots * -360 * needleScale);
        if (metterText)
            metterText.GetComponent<Text>().text = ((int)foots).ToString();
    }


    float getValue()
    {
        switch (metterType)
        {
            case MetterType.Altitude:
                return Plane.transform.position.y * 3.281f;
            case MetterType.Velocity:
                return Plane.GetSpeedInNautics();
            case MetterType.OilPressure:
                return 0;
                break;
            case MetterType.NozPosOpen:
                return 0;
                break;
            case MetterType.RPMPercent:
                return 0;
                break;
            case MetterType.Fuel:
                return 0;
                break;
            case MetterType.HydrolicPressure:
                return 0;
                break;
            default:
                return 0;
        }
    }
}

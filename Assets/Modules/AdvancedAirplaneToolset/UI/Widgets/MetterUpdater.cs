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

[ExecuteInEditMode]
public class MetterUpdater : PlaneComponent
{
    public GameObject metterText;

    public GameObject metterNeedle;

    public MetterType metterType = MetterType.Altitude;

    public float needleScale = 0.0001f;

    public float currentValue = 0;

    void Update()
    {
        float foots = getValue();
        metterNeedle.transform.localRotation = Quaternion.Euler(0, 0, foots * -360 * needleScale);
        if (metterText)
            metterText.GetComponent<Text>().text = ((int)foots).ToString();
    }


    float getValue()
    {
        if (!Plane)
            return currentValue;
        switch (metterType)
        {
            case MetterType.Altitude:
                return Plane.transform.position.y * 3.281f;
            case MetterType.Velocity:
                return Plane.GetSpeedInNautics();
            case MetterType.OilPressure:
                return 0;
            case MetterType.NozPosOpen:
                return 0;
            case MetterType.RPMPercent:
                return Plane.GetRpmPercent(0);
            case MetterType.Fuel:
                return 0;
            case MetterType.HydrolicPressure:
                return 0;
            default:
                return 0;
        }
    }
}

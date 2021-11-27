using UnityEngine;
using UnityEngine.UI;

// Type de propriétée utilisée
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

// Component chargé de mettre a jour les aiguilles et autres indicateurs dans le cockpit de l'avion
[ExecuteInEditMode]
public class MetterUpdater : PlaneComponent
{
    // Texte indiquant la valeur (optionnel)
    public GameObject metterText;
    // Aiguille
    public GameObject metterNeedle;
    // Type d'indicateur
    public MetterType metterType = MetterType.Altitude;
    // Multiplicateur de rotation par rapport à la valeur de base
    public float needleScale = 0.0001f;
    public float currentValue = 0;

    void Update()
    {
        float foots = getValue();
        metterNeedle.transform.localRotation = Quaternion.Euler(0, 0, foots * -360 * needleScale);
        if (metterText)
            metterText.GetComponent<Text>().text = ((int)foots).ToString();
    }


    // Retourne la valeur correspondante au type de l'indicateur
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

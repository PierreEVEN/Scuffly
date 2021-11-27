using UnityEngine;

// Action du bouton
public enum EButtonType
{
    None,
    AirAir,
    AirGround,
}

// Bouton sur lequel on peut appuyer, a placer dans le cockpit
public class Button : SwitchBase
{
    // Action du bouton
    public EButtonType buttonType;
    // Offset maximal du bouton pressé
    public Vector3 pushVector = new Vector3(0, 0, 0.005f);

    Vector3 initialRelativePos;

    // Temps de l'animation de pression sur le bouton
    float Timeline = 0;

    private void OnEnable()
    {
        initialRelativePos = transform.localPosition;
    }

    private void Update()
    {
        if (Timeline > 0)
        {
            Timeline -= Time.deltaTime * 5;

            // Joue l'animation de timeline
            transform.localPosition = initialRelativePos + pushVector * (0.5f - Mathf.Abs(Timeline - 0.5f)) * 2;
        }
        if (Timeline <= 0)
            transform.localPosition = initialRelativePos;
    }
    
    // Appel des differentes fonctions
    public override void Switch()
    {
        switch (buttonType)
        {
            case EButtonType.AirAir:
                WeaponSystem.AirAirMode();
                break;
            case EButtonType.AirGround:
                WeaponSystem.AirGroundMode();
                break;
            default:
                break;
        }
        // Joue la timeline depuis le debut
        Timeline = 1;
    }
}

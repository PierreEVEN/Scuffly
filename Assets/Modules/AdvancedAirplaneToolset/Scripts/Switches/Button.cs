using UnityEngine;

public enum EButtonType
{
    None,
    AirAir,
    AirGround,
    Cockpit,
}

/// <summary>
/// A simple pressable button
/// </summary>
public class Button : SwitchBase
{
    /// <summary>
    /// The button action
    /// </summary>
    public EButtonType buttonType;
    
    /// <summary>
    /// The max offset of the button when pushed
    /// </summary>
    public Vector3 pushVector = new Vector3(0, 0, 0.005f);

    /// <summary>
    /// Relative released pos to it's parent
    /// </summary>
    Vector3 initialRelativePos;

    /// <summary>
    /// Current press animation time
    /// </summary>
    float Timeline = 0;

    private void OnEnable()
    {
        initialRelativePos = transform.localPosition;
    }

    private void Update()
    {
        // If timeline is > 0 : play animation (animation_time = 1 - Timeline)
        if (Timeline > 0)
        {
            Timeline -= Time.deltaTime * 5;

            transform.localPosition = initialRelativePos + pushVector * (0.5f - Mathf.Abs(Timeline - 0.5f)) * 2;
        }
        if (Timeline <= 0)
            transform.localPosition = initialRelativePos;
    }
    
    public override void Switch()
    {
        // Call the desired event depending on the button type
        switch (buttonType)
        {
            case EButtonType.AirAir:
                WeaponSystem.AirAirMode();
                break;
            case EButtonType.AirGround:
                WeaponSystem.AirGroundMode();
                break;
            case EButtonType.None:
                break;
            case EButtonType.Cockpit:
                Plane.OpenCanopy = !Plane.OpenCanopy;
                break;
            default:
                break;
        }

        // Start the button animation
        Timeline = 1;
    }

    public override void Release()
    {
    }
}

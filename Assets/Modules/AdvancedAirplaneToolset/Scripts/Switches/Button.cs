using UnityEngine;

public enum EButtonType
{
    None,
    AirAir,
    AirGround,
}

public class Button : SwitchBase
{
    public EButtonType buttonType;
    Vector3 pushVector = new Vector3(0, 0, 0.005f);

    Vector3 initialRelativePos;

    float Timeline;

    private void OnEnable()
    {
        initialRelativePos = transform.localPosition;
    }

    private void Update()
    {
        if (Timeline > 0)
        {
            Timeline -= Time.deltaTime * 5;

            transform.localPosition = initialRelativePos + pushVector * (0.5f - Mathf.Abs(Timeline - 0.5f)) * 2;
            if (Timeline <= 0)
                transform.localPosition = initialRelativePos;
        }
    }

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
        Timeline = 1;
    }
}

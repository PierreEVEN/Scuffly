
using UnityEngine;

public enum RotatingSwitchStates
{
    None,
    FloodLights,
    HudLevel,
    RightScreen,
    LeftScreen,
    AntiColLights,
}

/// <summary>
/// A simple spinning button
/// </summary>
public class RotatingSwitch : SwitchBase
{
    /// <summary>
    /// The modified plane property
    /// </summary>
    public RotatingSwitchStates modifiedProperty = RotatingSwitchStates.None;

    /// <summary>
    /// Current value
    /// </summary>
    float value = 0;

    /// <summary>
    /// Is user currently holding the click to make the button spin
    /// </summary>
    bool isMoving = false;

    /// <summary>
    /// The value of the button before the user started to make it spin
    /// </summary>
    float initialValue;

    /// <summary>
    /// The max rotation
    /// </summary>
    public float range = 300;

    /// <summary>
    /// The camera direction, when the user started dragging the button
    /// </summary>
    Vector2 initialCameraDir;

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // Retrieve the offset of the camera between the moment where the user started to drag the button and the current lock vector
            Vector2 delta = new Vector2(Camera.main.transform.localRotation.eulerAngles.x, Camera.main.transform.localRotation.eulerAngles.y) - initialCameraDir;
            value = Mathf.Clamp(initialValue + (-delta.x + delta.y) / 10, 0, 1);

            // The update the property
            switch (modifiedProperty)
            {
                case RotatingSwitchStates.None:
                    break;
                case RotatingSwitchStates.FloodLights:
                    Plane.CockpitFloodLights = value;
                    break;
                case RotatingSwitchStates.HudLevel:
                    Plane.HudLightLevel = value;
                    break;
                case RotatingSwitchStates.RightScreen:
                    break;
                case RotatingSwitchStates.LeftScreen:
                    break;
                case RotatingSwitchStates.AntiColLights:
                    Plane.PositionLight = value;
                    break;
            }
        }
        else
        {
            // make the rotation of the button match the real plane value
            switch (modifiedProperty)
            {
                case RotatingSwitchStates.None:
                    break;
                case RotatingSwitchStates.FloodLights:
                    value = Plane.CockpitFloodLights;
                    break;
                case RotatingSwitchStates.HudLevel:
                    value = Plane.HudLightLevel;
                    break;
                case RotatingSwitchStates.RightScreen:
                    break;
                case RotatingSwitchStates.LeftScreen:
                    break;
                case RotatingSwitchStates.AntiColLights:
                    value = Plane.PositionLight;
                    break;
            }
        }

        // Update mesh rotation
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).localRotation = Quaternion.Euler(0, value * range, 0);
    }

    public override void Switch()
    {
        // We are starting draging the button
        isMoving = true;
        initialValue = value;
        initialCameraDir = new Vector2(Camera.main.transform.localRotation.eulerAngles.x, Camera.main.transform.localRotation.eulerAngles.y);
    }

    public override void Release()
    {
        // We released the button
        isMoving = false;
    }
}

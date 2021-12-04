
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

//@TODO AJOUTER LES POTENTIOMETRES
public class RotatingSwitch : SwitchBase
{
    public RotatingSwitchStates modifiedProperty = RotatingSwitchStates.None;

    float value = 0;

    bool isMoving = false;
    float initialValue;
    public float range = 300;
    Vector2 initialCameraDir;

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            Vector2 delta = new Vector2(Camera.main.transform.localRotation.eulerAngles.x, Camera.main.transform.localRotation.eulerAngles.y) - initialCameraDir;
            value = Mathf.Clamp(initialValue + (-delta.x + delta.y) / 10, 0, 1);
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

        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).localRotation = Quaternion.Euler(0, value * range, 0);
        }
    }

    public override void Switch()
    {
        isMoving = true;
        initialValue = value;
        initialCameraDir = new Vector2(Camera.main.transform.localRotation.eulerAngles.x, Camera.main.transform.localRotation.eulerAngles.y);
    }

    public override void Release()
    {
        isMoving = false;
    }
}

using AK.Wwise;
using UnityEngine;

/// <summary>
/// Widget that display a target on the HUD for the Aim9 missiles
/// </summary>
public class HUDWidgetIR : HUDComponent
{
    /// <summary>
    /// The sound telling if the sensor locked a target
    /// </summary>
    public AK.Wwise.Event PlayTone;
    public AK.Wwise.Event StopTone;
    public RTPC ToneRtpc;

    private void OnEnable()
    {
        PlayTone.Post(IrDetectorComponent.gameObject);
    }

    private void OnDisable()
    {
        if (IrDetectorComponent)
            StopTone.Post(IrDetectorComponent.gameObject);
    }

    private void Update()
    {
        // If a target is acquired by then the infrared sensor, the pitch of the search tone is moved up, and the aim circle is moved to the target
        if (IrDetectorComponent.acquiredTarget)
        {
            ToneRtpc.SetGlobalValue(100);
            transform.localPosition = HUD.WorldDirectionToScreenPosition((IrDetectorComponent.acquiredTarget.transform.position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2))) - Plane.transform.position);
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
        else
        {
            // else move it back at the center of the screen
            ToneRtpc.SetGlobalValue(0);
            transform.localPosition = HUD.WorldDirectionToScreenPosition(Plane.transform.forward);
            transform.localScale = Vector3.one;
        }
    }
}

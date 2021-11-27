using AK.Wwise;
using UnityEngine;

public class HUDWidgetIR : HUDComponent
{

    public float DetectionAngle = 2.5f;
    public float LoseAngle = 6;
    public float DetectionRange = 10000;

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
        if (IrDetectorComponent.acquiredTarget)
        {
            ToneRtpc.SetValue(IrDetectorComponent.gameObject, 100);
            transform.localPosition = HUD.WorldDirectionToScreenPosition((IrDetectorComponent.acquiredTarget.transform.position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2))) - Plane.transform.position);
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
        else
        {
            ToneRtpc.SetValue(IrDetectorComponent.gameObject, 0);
            transform.localPosition = HUD.WorldDirectionToScreenPosition(Plane.transform.forward);
            transform.localScale = Vector3.one;
        }
    }
}

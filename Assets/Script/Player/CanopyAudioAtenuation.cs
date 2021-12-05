using AK.Wwise;
using UnityEngine;

[RequireComponent(typeof(MobilePart))]
public class CanopyAudioAtenuation : PlaneComponent
{
    public RTPC CockpitLevelRTPC;

    // Update is called once per frame
    void Update()
    {
        CameraManager camera = PlayerManager.LocalPlayer.GetComponent<CameraManager>();
        if (PlayerManager.LocalPlayer.controlledPlane == Plane)
        {
            CockpitLevelRTPC.SetGlobalValue(camera.Indoor && !camera.IsFreeCamera() ? 100 - Mathf.Pow(GetComponent<MobilePart>().currentInput, 0.5f) * 100 : 0);
        }
    }
}

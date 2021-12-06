using AK.Wwise;
using UnityEngine;

[RequireComponent(typeof(MobilePart))]
public class CanopyAudioAtenuation : PlaneComponent
{
    public RTPC CockpitLevelRTPC;
    public RTPC CockpitCloseSpeed;
    public AK.Wwise.Event CockpitClosePlayEvent;
    public AK.Wwise.Event CockpitCloseStopEvent;

    float currentAudioLevel;
    bool isPlaying = false;
    // Update is called once per frame
    void Update()
    {
        CameraManager camera = PlayerManager.LocalPlayer.GetComponent<CameraManager>();
        if (PlayerManager.LocalPlayer.controlledPlane == Plane)
        {
            CockpitLevelRTPC.SetGlobalValue(camera.Indoor && !camera.IsFreeCamera() ? 100 - Mathf.Pow(GetComponent<MobilePart>().currentInput, 0.5f) * 100 : 0);
        }

        float audioLevel = 0;

        if (GetComponent<MobilePart>().currentInput < GetComponent<MobilePart>().desiredInput)
            audioLevel = 1;
        if (GetComponent<MobilePart>().currentInput > GetComponent<MobilePart>().desiredInput)
            audioLevel = -1;

        currentAudioLevel = Mathf.Lerp(currentAudioLevel, audioLevel, Time.deltaTime * 10);

        if (Mathf.Abs(currentAudioLevel) < 0.05f )
        {
            if (isPlaying)
            {
                isPlaying = false;
                CockpitCloseStopEvent.Post(gameObject);
            }
        }
        else
        {
            if (!isPlaying)
            {
                isPlaying = true;
                CockpitClosePlayEvent.Post(gameObject);
            }
        }
        CockpitCloseSpeed.SetValue(gameObject, currentAudioLevel * 50 + 50);
    }
}

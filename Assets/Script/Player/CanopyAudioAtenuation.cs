using AK.Wwise;
using UnityEngine;

/// <summary>
/// Handle the audio attenuation when closing the canopy. Also play a sound when closing / opening it
/// The close speed will influence the pitch and the volume (speed is between -1 and 1)
/// </summary>
[RequireComponent(typeof(MobilePart))]
public class CanopyAudioAtenuation : PlaneComponent
{
    /// <summary>
    /// Audio parameters
    /// </summary>
    public RTPC CockpitLevelRTPC;
    public RTPC CockpitCloseSpeed;
    public AK.Wwise.Event CockpitClosePlayEvent;
    public AK.Wwise.Event CockpitCloseStopEvent;

    /// <summary>
    /// Current close speed
    /// </summary>
    float currentCloseSpeed;
    bool isPlaying = false;
    // Update is called once per frame
    void Update()
    {
        // Update global exterior attenuation
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

        // Smooth the close speed
        currentCloseSpeed = Mathf.Lerp(currentCloseSpeed, audioLevel, Time.deltaTime * 10);

        // Play and stop the audio if volume < 0.05
        if (Mathf.Abs(currentCloseSpeed) < 0.05f )
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
        CockpitCloseSpeed.SetValue(gameObject, currentCloseSpeed * 50 + 50);
    }
}

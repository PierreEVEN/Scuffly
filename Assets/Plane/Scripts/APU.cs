using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APU : MonoBehaviour
{
    public float StartupDuration = 7.3f;
    public float ShutdownDuration = 7.3f;

    public AudioClip StartupAudio;
    public AudioClip IdleAudio;
    public AudioClip ShutdownAudio;

    private AudioSource ApuStartAudioSource;
    private AudioSource ApuIdleAudioSource;
    private AudioSource ApuShutdownAudioSource;

    private GameObject StartupContainer;
    private GameObject ShutdownContainer;

    float CurrentStartupPercent = 0.0f;

    bool IsEnabled = false;

    void Start()
    {
        StartupContainer = new GameObject("Startup container");
        StartupContainer.transform.parent = gameObject.transform;
        StartupContainer.transform.position = gameObject.transform.position;
        ShutdownContainer = new GameObject("Shutdown container");
        ShutdownContainer.transform.parent = gameObject.transform;
        ShutdownContainer.transform.position = gameObject.transform.position;

        ApuIdleAudioSource = gameObject.AddComponent<AudioSource>();
        ApuIdleAudioSource.spatialBlend = 1.0f;
        ApuIdleAudioSource.clip = IdleAudio;
        ApuIdleAudioSource.loop = true;

        ApuStartAudioSource = StartupContainer.AddComponent<AudioSource>();
        ApuStartAudioSource.spatialBlend = 1.0f;
        ApuStartAudioSource.clip = StartupAudio;

        ApuShutdownAudioSource = ShutdownContainer.AddComponent<AudioSource>();
        ApuShutdownAudioSource.spatialBlend = 1.0f;
        ApuShutdownAudioSource.clip = ShutdownAudio;
    }

    public void StartApu()
    {
        if (IsEnabled) return;

        CurrentStartupPercent = 0.0f;
        ApuStartAudioSource.Play();
        IsEnabled = true;
    }

    public void StopApu()
    {
        if (!IsEnabled) return;
        ApuShutdownAudioSource.Play();
        IsEnabled = false;
    }

    public bool IsReady()
    {
        return CurrentStartupPercent > 0.5f;
    }

    void Update()
    {
        ApuIdleAudioSource.volume = CurrentStartupPercent;
        if (CurrentStartupPercent < 0.01f && ApuIdleAudioSource.enabled)
            ApuIdleAudioSource.enabled = false;

        else if (!ApuIdleAudioSource.enabled)
        {
            ApuIdleAudioSource.enabled = true;
            ApuIdleAudioSource.Play();
        }
        CurrentStartupPercent = Mathf.Clamp(CurrentStartupPercent + (IsEnabled ? Time.deltaTime / StartupDuration : -Time.deltaTime / ShutdownDuration), 0, 1);
    }
}

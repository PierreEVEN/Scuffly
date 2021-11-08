using UnityEngine;
/**
 *  @Author : Pierre EVEN
 *  
 *  L'APU d'un avion est une generatrice electrique. Son fonctionnement est necessaire pour la mise en route du moteur
 *  
 */
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
        ApuIdleAudioSource.dopplerLevel = 0;
        ApuIdleAudioSource.minDistance = 20;
        ApuIdleAudioSource.clip = IdleAudio;
        ApuIdleAudioSource.loop = true;
        ApuIdleAudioSource.volume = 0.5f;

        ApuStartAudioSource = StartupContainer.AddComponent<AudioSource>();
        ApuStartAudioSource.spatialBlend = 1.0f;
        ApuStartAudioSource.minDistance = 20;
        ApuStartAudioSource.dopplerLevel = 0;
        ApuStartAudioSource.clip = StartupAudio;
        ApuStartAudioSource.volume = 0.5f;

        ApuShutdownAudioSource = ShutdownContainer.AddComponent<AudioSource>();
        ApuShutdownAudioSource.spatialBlend = 1.0f;
        ApuShutdownAudioSource.dopplerLevel = 0;
        ApuShutdownAudioSource.minDistance = 20;
        ApuShutdownAudioSource.clip = ShutdownAudio;
        ApuShutdownAudioSource.volume = 0.5f;
    }

    public void StartApu()
    {
        if (IsEnabled) return;

        // Allume l'APU. Joue le son d'allumage
        CurrentStartupPercent = 0.0f;
        if (ApuStartAudioSource)
            ApuStartAudioSource.Play();
        IsEnabled = true;
    }

    public void StopApu()
    {
        // Coupe l'APU : Joue le son d'etteinte.
        if (!IsEnabled) return;
        ApuShutdownAudioSource.Play();
        IsEnabled = false;
    }

    // Test si l'APU genere assez d'energie pour permettre le demarrage des systemes de l'avion necessitant une forte puissance electrique
    public bool IsReady()
    {
        return CurrentStartupPercent > 0.9f;
    }

    void Update()
    {
        // On fait moduler le volume du son "idle" en fonction de l'etat de fonctionnement de l'APU
        ApuIdleAudioSource.volume = Mathf.Clamp(CurrentStartupPercent * 0.5f - 0.1f, 0, 1);
        if (CurrentStartupPercent < 0.01f && ApuIdleAudioSource.enabled)
            ApuIdleAudioSource.enabled = false;
        else if (CurrentStartupPercent > 0.02f && !ApuIdleAudioSource.enabled)
        {
            ApuIdleAudioSource.enabled = true;
            ApuIdleAudioSource.Play();
        }
        // Met a jour l'etat courant de l'APU (= vitesse de rotation de l'alternateur)
        CurrentStartupPercent = Mathf.Clamp(CurrentStartupPercent + (IsEnabled ? Time.deltaTime / StartupDuration : -Time.deltaTime / ShutdownDuration), 0, 1);
    }
}

using UnityEngine;
/**
 *  @Author : Pierre EVEN
 *  
 *  L'APU d'un avion est une generatrice electrique. Son fonctionnement est necessaire pour la mise en route du moteur
 *  
 */
public class APU : PlaneComponent, IPowerProvider
{
    public float StartupDuration = 14;
    public float ShutdownDuration = 12;

    public AudioClip StartupAudio;
    public AudioClip IdleAudio;
    public AudioClip ShutdownAudio;

    private AudioSource apuStartAudioSource;
    private AudioSource apuIdleAudioSource;
    private AudioSource apuShutdownAudioSource;

    private GameObject startupContainer;
    private GameObject shutdownContainer;

    float currentStartupPercent = 0.0f;

    bool apuEnabled = false;

    float enoughPowerTimer = 0;


    void OnEnable()
    {
        if (!startupContainer)
        {
            startupContainer = new GameObject("Startup container");
            startupContainer.transform.parent = gameObject.transform;
            startupContainer.transform.position = gameObject.transform.position;
        }
        if (!shutdownContainer)
        {
            shutdownContainer = new GameObject("Shutdown container");
            shutdownContainer.transform.parent = gameObject.transform;
            shutdownContainer.transform.position = gameObject.transform.position;
        }
        if (!apuIdleAudioSource)
        {
            apuIdleAudioSource = gameObject.AddComponent<AudioSource>();
            apuIdleAudioSource.spatialBlend = 1.0f;
            apuIdleAudioSource.dopplerLevel = 0;
            apuIdleAudioSource.minDistance = 20;
            apuIdleAudioSource.clip = IdleAudio;
            apuIdleAudioSource.loop = true;
            apuIdleAudioSource.volume = 0.5f;
        }
        if (!apuStartAudioSource)
        {
            apuStartAudioSource = startupContainer.AddComponent<AudioSource>();
            apuStartAudioSource.spatialBlend = 1.0f;
            apuStartAudioSource.minDistance = 20;
            apuStartAudioSource.dopplerLevel = 0;
            apuStartAudioSource.clip = StartupAudio;
            apuStartAudioSource.volume = 0.5f;
        }
        if (!apuShutdownAudioSource)
        {
            apuShutdownAudioSource = shutdownContainer.AddComponent<AudioSource>();
            apuShutdownAudioSource.spatialBlend = 1.0f;
            apuShutdownAudioSource.dopplerLevel = 0;
            apuShutdownAudioSource.minDistance = 20;
            apuShutdownAudioSource.clip = ShutdownAudio;
            apuShutdownAudioSource.volume = 0.5f;
        }

        Plane.OnApuChange.AddListener(UpdateState);
        Plane.OnPowerSwitchChanged.AddListener(UpdateState);
        Plane.RegisterPowerProvider(this);
        if (Plane.initialApuSwitch)
        {
            currentStartupPercent = 1;
            apuEnabled = true;
        }
        UpdateState();
    }

    private void OnDisable()
    {
        Plane.OnApuChange.RemoveListener(UpdateState);
        Plane.OnPowerSwitchChanged.RemoveListener(UpdateState);
        StopApu();
    }

    private void OnGUI()
    {
        if (!Plane.EnableDebug)
            return;
        GUILayout.BeginArea(new Rect(200, 0, 200, 100));
        GUILayout.Label("APU Enabled : " + apuEnabled);
        GUILayout.Label("APU IN : " + Plane.GetCurrentPower() + " / 5");
        GUILayout.Label("APU OUT : " + currentStartupPercent * 40);
        GUILayout.EndArea();
    }

    void UpdateState()
    {
        // requiere 5 KVA pour demarrer / produit 40 kva
        if (Plane.EnableAPU && Plane.GetCurrentPower() > 5 && enoughPowerTimer < 10) // si le moteur de l'avion fournit assez d'energie, couper l'APU car inutile
            StartApu();
        else
        {
            enoughPowerTimer = 0;
            StopApu();
            Plane.EnableAPU = false;
        }
    }

    public void StartApu()
    {
        if (apuEnabled) return;

        // Allume l'APU. Joue le son d'allumage
        if (apuStartAudioSource)
            apuStartAudioSource.Play();
        apuEnabled = true;
    }

    public void StopApu()
    {
        // Coupe l'APU : Joue le son d'etteinte.
        if (!apuEnabled) return;
        apuShutdownAudioSource.Play();
        apuEnabled = false;
    }

    void Update()
    {
        // On fait moduler le volume du son "idle" en fonction de l'etat de fonctionnement de l'APU
        apuIdleAudioSource.volume = Mathf.Clamp(currentStartupPercent * 0.5f - 0.1f, 0, 1);
        if (currentStartupPercent < 0.01f && apuIdleAudioSource.enabled)
            apuIdleAudioSource.enabled = false;
        else if (currentStartupPercent > 0.02f && !apuIdleAudioSource.enabled)
        {
            apuIdleAudioSource.enabled = true;
            apuIdleAudioSource.Play();
        }
        // Met a jour l'etat courant de l'APU (= vitesse de rotation de l'alternateur)
        currentStartupPercent = Mathf.Clamp(currentStartupPercent + (apuEnabled ? Time.deltaTime / StartupDuration : -Time.deltaTime / ShutdownDuration), 0, 1);

        // verifie si l'apu peut etre eteint
        if (apuEnabled)
        {
            if (Plane.GetCurrentPower() > 80)
                enoughPowerTimer += Time.deltaTime;
            else
                enoughPowerTimer = 0;
            UpdateState();
        }

    }

    public float GetPower()
    {
        return currentStartupPercent * 40;
    }
}

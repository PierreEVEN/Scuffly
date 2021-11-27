using UnityEngine;
/**
 *  @Author : Pierre EVEN
 *  
 *  L'APU d'un avion est une generatrice electrique. Son fonctionnement est necessaire pour la mise en route du moteur
 *  
 */
[RequireComponent(typeof(AudioEngine))]
public class APU : PlaneComponent, IPowerProvider
{
    public float StartupDuration = 14;
    public float ShutdownDuration = 12;

    float currentStartupPercent = 0.0f;

    bool apuEnabled = false;

    float enoughPowerTimer = 0;

    AudioEngine audioEngine;
    void OnEnable()
    {
        Plane.OnApuChange.AddListener(UpdateState);
        Plane.OnPowerSwitchChanged.AddListener(UpdateState);
        Plane.RegisterPowerProvider(this);
        if (Plane.initialApuSwitch)
        {
            currentStartupPercent = 1;
            apuEnabled = true;
        }
        UpdateState();

        audioEngine = GetComponent<AudioEngine>();
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

        apuEnabled = true;
    }

    public void StopApu()
    {
        // Coupe l'APU : Joue le son d'etteinte.
        if (!apuEnabled) return;
        apuEnabled = false;
    }

    void Update()
    {
        // Met a jour l'etat courant de l'APU (= vitesse de rotation de l'alternateur)
        currentStartupPercent = Mathf.Clamp(currentStartupPercent + (apuEnabled ? Time.deltaTime / StartupDuration : -Time.deltaTime / ShutdownDuration), 0, 1);

        audioEngine.SetStartPercent(currentStartupPercent);

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

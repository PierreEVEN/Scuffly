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
    // Durees de mise en route et d'extinction
    public float StartupDuration = 14;
    public float ShutdownDuration = 12;

    // Etat du demarrage (0 - 1)
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
        apuEnabled = false;
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

    // Met a jour l'etat de l'APU : desactive si l'energie est trop basse, ou qu'elle est suffisante pour ne plus necessiter d'apoint (
    void UpdateState()
    {
        // requiere 5 KVA pour demarrer / produit 40 kva
        if (Plane.EnableAPU && Plane.GetCurrentPower() > 5 && enoughPowerTimer < 10) // si le moteur de l'avion fournit assez d'energie, couper l'APU car inutile
            apuEnabled = true;
        else
        {
            enoughPowerTimer = 0;
            apuEnabled = false;
            Plane.EnableAPU = false;
        }
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

    // Interface IPowerProvider : energie produite par l'APU
    public float GetPower()
    {
        return currentStartupPercent * 40;
    }
}

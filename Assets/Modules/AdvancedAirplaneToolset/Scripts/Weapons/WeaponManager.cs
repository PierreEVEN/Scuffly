
using UnityEngine;
using UnityEngine.Events;

public enum WeaponMode
{
    None,
    Canon, // We are using the canon
    Pod_Air, // We are using the air air weapons attached to the pods
    Pod_Ground, // We are using the air ground weapons attached to the pods
}

// Gestion de l'armement de l'avion. La sécurité d'armement doit etre levée pour pouvoir l'utiliser.
/// <summary>
/// Handle the different kind of weapons of the plane
/// </summary>
public class WeaponManager : PlaneComponent
{
    /// <summary>
    /// What kind of wepaon we are currently using
    /// </summary>
    WeaponMode weaponMode = WeaponMode.None;
    public WeaponMode CurrentWeaponMode { get { return weaponMode; } }

    /// <summary>
    /// What kind of pod item is currently used
    /// </summary>
    PodItemType currentPodType = PodItemType.Missile_IR;
    public PodItemType CurrentSelectedWeaponType { get { return currentPodType; } }

    /// <summary>
    /// Disable the weapon system if power is not set to ON
    /// </summary>
    public bool IsEnabled
    {
        get { return Plane.MainPower && toggleWeaponOn; }
    }

    /// <summary>
    /// Is weapon safety ON
    /// </summary>
    bool toggleWeaponOn = false;
    public bool IsToggledOn
    {
        get { return toggleWeaponOn; }
        set { toggleWeaponOn = value; }
    }

    // @TODO improve weapon systeme
    [HideInInspector]
    public UnityEvent OnSwitchWeaponMode = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnSwitchToNextPod = new UnityEvent();

    bool isShooting = false;
    bool beginShoot = false;

    WeaponPod selectedPod = null;

    [HideInInspector]
    public GameObject acquiredTarget;

    /// <summary>
    /// Start pressing the fire button
    /// </summary>
    public void BeginShoot()
    {
        isShooting = true;
        beginShoot = true;
    }

    /// <summary>
    /// Release the fire button
    /// </summary>
    public void EndShoot()
    {
        isShooting = false;
    }

    private void Update()
    {
        if (!isShooting || !IsToggledOn)
            return;


        switch (weaponMode)
        {
            case WeaponMode.Canon:
                FireCanon(); // Fire with the cannon while we are pressing the fire button
                break;
            case WeaponMode.Pod_Air:
            case WeaponMode.Pod_Ground:
                if (beginShoot)
                    FirePod(); // Only shoot missile when we started pressing the fire button
                break;
        }

        beginShoot = false;
    }

    /// <summary>
    /// Shoot the current selected missile, and select the next note
    /// </summary>
    void FirePod()
    {
        if (!selectedPod || !selectedPod.attachedPodItem)
            SwitchToNextPod();

        if (selectedPod)
        {
            selectedPod.Shoot(acquiredTarget);
            SwitchToNextPod();
        }
    }

    void FireCanon()
    {
        Debug.Log("not implemented yet");
    }

    /// <summary>
    /// Select the nex pod that correspond to the current weapon mode and contains a weapon
    /// </summary>
    public void SwitchToNextPod()
    {
        var pods = GetComponentsInChildren<WeaponPod>();
        int selectedPodIndex = 0;
        for (int i = 0; i < pods.Length; ++i)
        {
            if (pods[i] == selectedPod || selectedPod == null)
            {
                selectedPodIndex = i;
                break;
            }
        }

        for (int i = selectedPodIndex; (i + 1) % pods.Length != selectedPodIndex; i = (i + 1) % pods.Length)
        {
            if (pods[i].attachedPodItem && pods[i].attachedPodItem.podItemType == currentPodType)
            {
                // We found a pod that contains a weapon that correspond to the current weapon mode, so we select it.
                selectedPod = pods[i];
                OnSwitchToNextPod.Invoke();
                return;
            }
        }
        // Failed to find a valid weapon
        selectedPod = null;
        OnSwitchToNextPod.Invoke();
    }

    /// <summary>
    /// Switch to air ground weapon mode, or none if it was already the case
    /// </summary>
    public void AirGroundMode()
    {
        weaponMode = weaponMode == WeaponMode.Pod_Ground ? WeaponMode.None : WeaponMode.Pod_Ground;
        SwitchToNextPod();

        OnSwitchWeaponMode.Invoke();
    }

    /// <summary>
    /// Switch to air air weapon mode, or none if it was already the case
    /// </summary>
    public void AirAirMode()
    {
        weaponMode = weaponMode == WeaponMode.Pod_Air ? WeaponMode.None : WeaponMode.Pod_Air;
        SwitchToNextPod();
        OnSwitchWeaponMode.Invoke();
    }

    /// <summary>
    /// Switch to canon weapon mode, or none if it was already the case
    /// </summary>
    public void SwitchToCanon()
    {
        weaponMode = weaponMode == WeaponMode.Canon ? WeaponMode.None : WeaponMode.Canon;
        OnSwitchWeaponMode.Invoke();
    }
}

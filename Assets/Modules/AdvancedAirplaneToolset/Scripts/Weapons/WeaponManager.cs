
using UnityEngine;
using UnityEngine.Events;

public enum WeaponMode
{
    None,
    Canon,
    Pod_Air,
    Pod_Ground,
}

// Gestion de l'armement de l'avion. La sécurité d'armement doit etre levée pour pouvoir l'utiliser.
public class WeaponManager : PlaneComponent
{
    bool toggleWeaponOn = false;
    // Mode d'armement selectionnes (air / sol / type de pod etc...)
    WeaponMode weaponMode = WeaponMode.None;
    PodItemType currentPodType = PodItemType.Missile_IR;
    // Getter : l'armement n'est pas active s'il n'y a pas d'energie dans l'avion
    public bool IsEnabled
    {
        get { return Plane.MainPower && toggleWeaponOn; }
    }

    public WeaponMode CurrentWeaponMode { get { return weaponMode; } }
    public PodItemType CurrentSelectedWeaponType { get { return currentPodType; } }

    // Etat de la securite d'armement
    public bool IsToggledOn
    {
        get { return toggleWeaponOn; }
        set { 
            toggleWeaponOn = value;
        }
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

    public void BeginShoot()
    {
        isShooting = true;
        beginShoot = true;
    }

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
                FireCanon();
                break;
            case WeaponMode.Pod_Air:
            case WeaponMode.Pod_Ground:
                if (beginShoot)
                    FirePod();
                break;
        }

        beginShoot = false;
    }

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
                selectedPod = pods[i];
                OnSwitchToNextPod.Invoke();
                return;
            }
        }
        selectedPod = null;
        OnSwitchToNextPod.Invoke();
    }

    public void AirGroundMode()
    {
        weaponMode = weaponMode == WeaponMode.Pod_Ground ? WeaponMode.None : WeaponMode.Pod_Ground;
        SwitchToNextPod();

        OnSwitchWeaponMode.Invoke();
    }

    public void AirAirMode()
    {
        weaponMode = weaponMode == WeaponMode.Pod_Air ? WeaponMode.None : WeaponMode.Pod_Air;
        SwitchToNextPod();
        OnSwitchWeaponMode.Invoke();
    }
    public void SwitchToCanon()
    {
        weaponMode = weaponMode == WeaponMode.Canon ? WeaponMode.None : WeaponMode.Canon;
        OnSwitchWeaponMode.Invoke();
    }
}

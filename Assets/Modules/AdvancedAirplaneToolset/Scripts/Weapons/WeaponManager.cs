
using UnityEngine;
using UnityEngine.Events;

public enum WeaponMode
{
    None,
    Canon,
    Pod_Air,
    Pod_Ground,
}

public class WeaponManager : PlaneComponent
{
    bool toggleWeaponOn = false;
    WeaponMode weaponMode = WeaponMode.None;
    PodItemType currentPodType = PodItemType.Missile_IR;
    public bool IsEnabled
    {
        get { return Plane.MainPower && toggleWeaponOn; }
    }

    public WeaponMode CurrentWeaponMode { get { return weaponMode; } }
    public PodItemType CurrentSelectedWeaponType { get { return currentPodType; } }

    public bool IsToggledOn
    {
        get { return toggleWeaponOn; }
        set { 
            toggleWeaponOn = value;
        }
    }

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

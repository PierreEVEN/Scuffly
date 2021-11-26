
using UnityEngine;
using UnityEngine.Events;

public enum WeaponMode
{
    Canon,
    Pod_Air,
    Pod_Ground,
}

public class WeaponManager : PlaneComponent
{
    bool toggleWeaponOn = false;
    WeaponMode weaponMode = WeaponMode.Pod_Air;
    PodItemType currentPodType = PodItemType.Missile_IR;
    public bool IsEnabled
    {
        get { return Plane.MainPower && toggleWeaponOn; }
    }

    public bool IsToggledOn
    {
        get { return toggleWeaponOn; }
        set { 
            toggleWeaponOn = value;
            Debug.Log("set weapon to " + value);
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

    public void SwitchToAirGround()
    {
        Debug.Log("switch to airground ");
        weaponMode = WeaponMode.Pod_Ground;
        SwitchToNextPod();

        OnSwitchWeaponMode.Invoke();
    }

    void UpdateWeaponMode()
    {
        if (weaponMode == WeaponMode.Pod_Air)
        {
            switch (currentPodType)
            {
                case PodItemType.Bomb:
                case PodItemType.Maverick:
                    currentPodType = PodItemType.Missile_IR;
                    break;
            }
        }
        else if (weaponMode == WeaponMode.Pod_Ground)
        {

        }

    public void SwitchToAirAir()
    {
        Debug.Log("switch to airair ");
        weaponMode = WeaponMode.Pod_Air;
        SwitchToNextPod();
        OnSwitchWeaponMode.Invoke();
    }
    public void SwitchToCanon()
    {
        Debug.Log("switch to canon ");
        weaponMode = WeaponMode.Canon;
        OnSwitchWeaponMode.Invoke();
    }
}

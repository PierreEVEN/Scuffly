using MLAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

/// <summary>
/// Interface allowing plane component to produce engergy
/// </summary>
public interface IPowerProvider
{
    /// <summary>
    ///  Produced power in Kva
    /// </summary>
    /// <returns></returns>
    public float GetPower();
}

public enum PlaneTeam
{
    Red,
    Blue,
}

/// <summary>
/// Base class of an aircraft (must be the root of the aircraft object)
/// 
/// Handle the plane inputs, the power, and give some informations about the current state of the plane
/// 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlaneActor : NetworkBehaviour
{
    /// <summary>
    /// Define manually the gravity center of the aircraft
    /// </summary>
    public Vector3 massCenter = new Vector3(0, 0, 0);

    /// <summary>
    /// Initial state of each property of the plane when spawned
    /// </summary>
    public bool initialThrottleNotch = false;
    public bool initialApuSwitch = false;
    public bool initialRetractGear = false;
    public bool initialParkingBrakes = true;
    public bool initialPower = false;
    public bool initialOpenCanopy = true;
    public float initialHudLightLevel = 0;
    public float initialPositionLight = 0;
    /// <summary>
    /// Initially throw the plane forward
    /// </summary>
    public float initialSpeed = 0;

    /// <summary>
    /// A list of spawned planes
    /// </summary>
    public static List<PlaneActor> PlaneList = new List<PlaneActor>();

    /// <summary>
    /// Current states of the plane
    /// </summary>
    private bool throttleNotch = false; // Is throttle notch of the gaz handle set to ON
    private bool apuSwitch = false; // Is apu switch enabled
    private bool retractGear = false; // Is gear switch set to retract
    private bool parkingBrakes = true; // Is parking brake switch set to ON
    private bool brakes = false; // Is brake key pressed
    private bool power = false; // Is main power switch set to on
    private bool canopy = true; // Is canopy opened switch set to Open
    private bool landingLights = false; // Is landing light set to ON
    private float floodLight = 0.1f; // Flood light brightness
    private float positionLight = 0; // Position light brightness
    private float hudLight = 0; // Hud light brightness

    /// <summary>
    /// Event called when plane state is changed
    /// </summary>
    [HideInInspector]
    public UnityEvent OnGearChange = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnThrottleNotchChange = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnApuChange = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnBrakesChange = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnCanopyChange = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnCockpitFloodlightChanged = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnPositionLightChanged = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnHudLightChanged = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnPowerSwitchChanged = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnGlobalPowerChanged = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnLandingLightsChanged = new UnityEvent();

    /// <summary>
    /// A list of component that are providing power to the plane
    /// </summary>
    private List<IPowerProvider> powerProviders = new List<IPowerProvider>();

    /// <summary>
    /// The current produced power
    /// </summary>
    private float currentEnginePower;
    /// <summary>
    /// Get the currently available electric power
    /// </summary>
    /// <returns></returns>
    public float GetCurrentPower()
    {
        return currentEnginePower;
    }


    /// <summary>
    /// Rigidbody of the plane
    /// </summary>
    public Rigidbody Physics { get { return planePhysic; } }
    Rigidbody planePhysic;

    /// <summary>
    /// Event called when the plane is destroyed
    /// </summary>
    [HideInInspector]
    public UnityEvent<PlaneActor> OnDestroyed = new UnityEvent<PlaneActor>();

    /// <summary>
    /// The gameObject spawned when the aircraft is destroyed
    /// </summary>
    public GameObject explosionObject;

    /// <summary>
    /// The last player or AI responsible for dmaging the plane
    /// </summary>
    [HideInInspector]
    public GameObject LastDamageInstigator;

    /// <summary>
    /// The object containing all the cockpit stuff.
    /// The cockpit is hidden when the player leave the plane
    /// </summary>
    public GameObject cockpitObject;

    /// <summary>
    /// The team of the plane (used for AIs behaviour)
    /// </summary>
    public PlaneTeam planeTeam = PlaneTeam.Blue;

    /// <summary>
    /// Register a new component that will provide power to the plane
    /// </summary>
    /// <param name="provider"></param>
    public void RegisterPowerProvider(IPowerProvider provider)
    {
        if (!powerProviders.Contains(provider))
            powerProviders.Add(provider);
    }

    /// <summary>
    /// Is debug mode enabled
    /// </summary>
    public bool EnableDebug { get { return false; } }

    /************************
     * Getter and setter for each parameter of the plane
     ************************/

    public bool MainPower
    {
        get { return power; }
        set
        {
            if (value != power)
            {
                UpdatePlanePower();
                power = value;
                OnPowerSwitchChanged.Invoke();
            }
        }
    }
    public bool ThrottleNotch
    {
        get { return throttleNotch; }
        set
        {
            if (value != throttleNotch)
            {
                UpdatePlanePower();
                throttleNotch = value;
                OnThrottleNotchChange.Invoke();
            }
        }
    }
    public bool EnableAPU
    {
        get { return apuSwitch; }
        set
        {
            if (value != apuSwitch)
            {
                UpdatePlanePower();
                apuSwitch = value;
                OnApuChange.Invoke();
            }
        }
    }
    public bool LandingLights
    {
        get { return landingLights; }
        set
        {
            if (value != landingLights)
            {
                landingLights = value;
                OnLandingLightsChanged.Invoke();
            }
        }
    }
    public bool RetractGear
    {
        get { return retractGear; }
        set
        {
            if (value != retractGear)
            {
                UpdatePlanePower();
                retractGear = value;
                OnGearChange.Invoke();
            }
        }
    }
    public bool ParkingBrakes
    {
        get { return parkingBrakes; }
        set
        {
            if (value != parkingBrakes)
            {
                UpdatePlanePower();
                parkingBrakes = value;
                OnBrakesChange.Invoke();
            }
        }
    }
    public bool Brakes
    {
        get { return brakes; }
        set
        {
            if (value != brakes)
            {
                UpdatePlanePower();
                brakes = value;
                OnBrakesChange.Invoke();
            }
        }
    }
    public bool OpenCanopy
    {
        get { return canopy; }
        set
        {
            if (value != canopy)
            {
                canopy = value;
                OnCanopyChange.Invoke();
            }
        }
    }    
    public float CockpitFloodLights
    {
        get { return floodLight; }
        set
        {
            if (value != floodLight)
            {
                floodLight = Mathf.Clamp01(value);
                OnCockpitFloodlightChanged.Invoke();
            }
        }
    }
    public float PositionLight
    {
        get { return positionLight; }
        set
        {
            if (value != positionLight)
            {
                positionLight = Mathf.Clamp01(value);
                OnPositionLightChanged.Invoke();
            }
        }
    }
    public float HudLightLevel
    {
        get { return hudLight; }
        set
        {
            if (value != hudLight)
            {
                hudLight = Mathf.Clamp01(value);
                OnHudLightChanged.Invoke();
            }
        }
    }


    void Start()
    {
        planePhysic = gameObject.GetComponent<Rigidbody>();
        // We set all the current values from the initial default values
        planePhysic.velocity = transform.forward * initialSpeed;
        EnableAPU = initialApuSwitch;
        ThrottleNotch = initialThrottleNotch;
        RetractGear = initialRetractGear;
        ParkingBrakes = initialParkingBrakes;
        MainPower = initialPower;
        OpenCanopy = initialOpenCanopy;
        HudLightLevel = initialHudLightLevel;
        PositionLight = initialPositionLight;
    }
    
    private void OnEnable()
    {
        PlaneList.Add(this);
    }
    private void OnDisable()
    {
        OnDestroyed.Invoke(this);
        PlaneList.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        planePhysic.centerOfMass = massCenter;

        float ro = 1.25f;
        float liftCoef = 1f;
        float surface = 27.87f;

        // Artificially add a lift coefficient to compensate the lack of lift in the current aerodynamic simulation
        foreach (var part in GetComponentsInChildren<Rigidbody>())
        {
            float velocity = Mathf.Abs(transform.InverseTransformDirection(part.velocity).z);

            // Common lift calculation
            part.AddForce(transform.up * 0.5f * ro * liftCoef * surface * velocity * velocity * Time.deltaTime);
        }

        UpdatePlanePower();
    }

    /// <summary>
    /// Fetch all the power provided by parts attached to the plane to compute the total power
    /// </summary>
    void UpdatePlanePower()
    {
        // Update power
        float newPower = MainPower ? 20 : 0;
        foreach (var provider in powerProviders)
        {
            newPower += provider.GetPower();
        }
        if (newPower != currentEnginePower)
        {
            currentEnginePower = newPower;
            OnGlobalPowerChanged.Invoke();
        }
    }

    private void OnGUI()
    {
        if (!EnableDebug)
            return;
        GUILayout.BeginArea(new Rect(0, 0, 200, 100));
        GUILayout.Label("PLANE POWER : " + currentEnginePower);
        GUILayout.Label("PowerSwitch : " + MainPower);
        GUILayout.Label("Gear : " + !RetractGear);
        GUILayout.Label("Brakes : " + ParkingBrakes);
        GUILayout.Label("Canopy : " + OpenCanopy);
        GUILayout.EndArea();
    }

    /*********************************
     * Axis and input controll
     *********************************/

    /// <summary>
    /// Current thrust input value
    /// </summary>
    [HideInInspector]
    public float ThrustInput = 0;

    /// <summary>
    /// Current pitch input value
    /// </summary>
    [HideInInspector]
    public float PitchInput = 0;

    /// <summary>
    /// Current yaw input value
    /// </summary>
    [HideInInspector]
    public float YawInput = 0;

    /// <summary>
    /// Current roll input value
    /// </summary>
    [HideInInspector]
    public float RollInput = 0;

    /// <summary>
    /// Change the current thrust input of the plane
    /// </summary>
    /// <param name="input"></param>
    public void SetThrustInput(float input)
    {
        foreach (var thruster in GetComponentsInChildren<Thruster>())
            thruster.setThrustInput(input);

        ThrustInput = input;
    }

    /// <summary>
    /// Change the current pitch input of the plane
    /// </summary>
    /// <param name="input"></param>
    public void SetPitchInput(float input)
    {
        PitchInput = Mathf.Clamp(input, -1, 1);
    }

    /// <summary>
    /// Change the current yaw input of the plane
    /// </summary>
    /// <param name="input"></param>
    public void SetYawInput(float input)
    {
        YawInput = Mathf.Clamp(input, -1, 1);
    }

    /// <summary>
    /// Change the current roll input of the plane
    /// </summary>
    /// <param name="input"></param>
    public void SetRollInput(float input)
    {
        RollInput = Mathf.Clamp(input, -1, 1);
    }

    /// <summary>
    /// Fire action of the plane
    /// </summary>
    /// <param name="target"></param>
    public void Shoot(GameObject target)
    {
        foreach (var part in GetComponentsInChildren<WeaponPod>())
        { // @todo handle weapon selection
            if (part.attachedPodItem)
            {
                part.Shoot(target);
                return;
            }
        }
    }

    /***************************************
     * Helpers : get informations about the plane
     ***************************************/

    /// <summary>
    /// Current plane forward velocity in m/s
    /// </summary>
    /// <returns></returns>
    public float GetSpeedInMetters()
    {
        return transform.InverseTransformDirection(planePhysic.velocity).z;
    }

    /// <summary>
    /// Current plane forward velocity in kt
    /// </summary>
    /// <returns></returns>
    public float GetSpeedInNautics()
    {
        return transform.InverseTransformDirection(planePhysic.velocity).z * 1.94384519992989f;
    }

    /// <summary>
    /// Current plane current attitude in degrees
    /// </summary>
    /// <returns></returns>
    public float GetAttitude()
    {
        return Mathf.Asin(transform.forward.y) / Mathf.PI * 180;
    }

    /// <summary>
    /// Current plane current roll in degrees
    /// </summary>
    /// <returns></returns>
    public float GetRoll()
    {
        return (transform.rotation.eulerAngles.z % 360 + 180 + 360) % 360 - 180;
    }

    /// <summary>
    /// Current heading of the aircraft in degrees
    /// </summary>
    /// <returns></returns>
    public float GetHeading()
    {
        return transform.rotation.eulerAngles.y;
    }

    /// <summary>
    /// Get altitude from the sea level in foots
    /// </summary>
    /// <returns></returns>
    public float GetAltitudeInFoots()
    {
        return transform.position.y * 3.281f;
    }

    /// <summary>
    /// When the aircraft hit an object
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // Detect if we hit an other airplane or simply we got hit by a missile. If it is the case, set it at the last damage instigator
        GameObject newDamageInstigator = null;
        PlaneActor contactPlane = collision.gameObject.GetComponentInParent<PlaneActor>();
        PodItem contactPodItem = collision.gameObject.GetComponentInParent<PodItem>();
        if (contactPlane)
            newDamageInstigator = contactPlane.gameObject;
        else if (contactPodItem && contactPodItem.owner)
            newDamageInstigator = contactPodItem.owner;
        if (newDamageInstigator)
            LastDamageInstigator = newDamageInstigator;

        // Apply damages to every part of the plane
        foreach (var component in GetComponentsInChildren<DamageableComponent>())
        {
            Vector3[] points = new Vector3[collision.contactCount];

            for (int i = 0; i < collision.contactCount; ++i)
                points[i] = collision.contacts[i].point;

            component.ApplyDamageAtLocation(points, 0.5f, collision.impulse.magnitude / 200, null);
        }

        // If the impact was a really severe one, destroy the plane
        for (int i = 0; i < collision.contactCount; ++i)
        {
            if (collision.GetContact(i).thisCollider.gameObject == gameObject)
            {
                if (collision.impulse.magnitude > 20000)
                {
                    OnExplode();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Make the plane explode - boom
    /// </summary>
    void OnExplode()
    {
        // Disable physics
        planePhysic.isKinematic = true;
        transform.position = transform.position - new Vector3(0, 8, 0);

        // Spawn the explosion visual (it will be destroyed in 50 seconds)
        if (explosionObject)
        {
            GameObject fxContainer = Instantiate(explosionObject);
            fxContainer.transform.position = transform.position;
            Destroy(fxContainer, 50);
        }
        // Destroy the plane
        OnDestroyed.Invoke(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Is cockpit hidden or visible
    /// </summary>
    bool? indoorEnabled = null;

    /// <summary>
    /// Show or hide the cockpit and all the interior stuffs
    /// </summary>
    /// <param name="enable"></param>
    public void EnableIndoor(bool enable)
    {
        if (indoorEnabled == enable)
            return;

        if (!cockpitObject)
            return;

        cockpitObject.SetActive(enable);
    }

    /// <summary>
    /// A list of thruster available on the aircraft
    /// </summary>
    List<Thruster> thrusterList = new List<Thruster>();
    public void RegisterThruster(Thruster thruster)
    {
        thrusterList.Add(thruster);
    }
    public void UnRegisterThruster(Thruster thruster)
    {
        thrusterList.Remove(thruster);
    }

    /// <summary>
    /// Get current oil pressure (not implemented)
    /// </summary>
    /// <returns></returns>
    public float GetOilPressure()
    {
        return 0;
    }

    /// <summary>
    /// Get the current rotation per minute of the thruster number 'index'
    /// </summary>
    /// <param name="thrusterIndex"></param>
    /// <returns></returns>
    public float GetRpmPercent(int thrusterIndex)
    {
        if (thrusterList.Count <= thrusterIndex)
            return 0;
        return thrusterList[thrusterIndex].ThrottlePercent * 0.8f + thrusterList[thrusterIndex].engineStartupPercent * 20f;
    }

    /// <summary>
    /// Get the current openning of the intake (not implemented yet)
    /// </summary>
    /// <param name="thrusterIndex"></param>
    /// <returns></returns>
    public float GetNozeOpenning()
    {
        return 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(massCenter), 1);
    }

    /// <summary>
    /// The radar of the plane
    /// </summary>
    Radar planeRadar;
    public Radar RadarComponent
    {
        get
        {
            if (!planeRadar)
                planeRadar = GetComponentInChildren<Radar>();
            return planeRadar;
        }
    }

    /// <summary>
    /// The weapons stuffs have been moved from this class to another one to simplify the system
    /// </summary>
    WeaponManager weaponSystem;
    public WeaponManager WeaponSystem
    {
        get
        {
            if (!weaponSystem)
                weaponSystem = GetComponentInChildren<WeaponManager>();
            return weaponSystem;
        }
    }
}
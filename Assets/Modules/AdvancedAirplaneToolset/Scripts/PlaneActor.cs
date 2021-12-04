using MLAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

// Interface a ajouter a un component capable de produire de l'energie
public interface IPowerProvider
{
    public float GetPower(); // Energie produite en KVA
}

/*
 * Class definissant un avion. (a placer a la racine du GameObject correspondant)
 * 
 * Pour fonctionner, l'avion a besoin d'une source d'energie ( ex : batteries qu'il faut d'abord allumer )
 * Le moteur et les systemes hydrauliques ne pourront etre allume que si l'APU est fonctionnel.
 * 
 * //@TODO : cleanup PlaneActor
 * 
 */
public enum PlaneTeam
{
    Red,
    Blue,
}

[RequireComponent(typeof(Rigidbody))]
public class PlaneActor : NetworkBehaviour
{
    // Definis le centre de gravite physique de l'avion
    public Vector3 massCenter = new Vector3(0, 0, 0);

    // Etat initial de l'avion a sa creation / valeurs par defaut
    public bool initialThrottleNotch = false;
    public bool initialApuSwitch = false;
    public bool initialRetractGear = false;
    public bool initialParkingBrakes = true;
    public float initialSpeed = 0;
    public bool initialPower = false;
    public bool initialOpenCanopy = true;
    public float initialHudLightLevel = 0;
    public float initialPositionLight = 0;

    Rigidbody planePhysic;
    private float currentEnginePower;

    public Rigidbody Physics { get { return planePhysic; } }

    // Etat courant de l'avion
    private bool throttleNotch = false;
    private bool apuSwitch = false;
    private bool retractGear = false;
    private bool parkingBrakes = true;
    private bool brakes = false;
    private bool power = false;
    private bool canopy = true;
    private bool landingLights = false;
    private float floodLight = 0.1f;
    private float positionLight = 0;
    private float hudLight = 0;

    // Active ou desactive des fonctionnalites de l'avion
    [HideInInspector]
    public UnityEvent OnGearChange = new UnityEvent(); // train rentre / sortis
    [HideInInspector]
    public UnityEvent OnThrottleNotchChange = new UnityEvent(); // manette des gaz activee / bloquee
    [HideInInspector]
    public UnityEvent OnApuChange = new UnityEvent(); // apu active / bloque
    [HideInInspector]
    public UnityEvent OnBrakesChange = new UnityEvent(); // freins actives / relaches
    [HideInInspector]
    public UnityEvent OnCanopyChange = new UnityEvent(); // ouvrir / fermer le cockpit
    [HideInInspector]
    public UnityEvent OnCockpitFloodlightChanged = new UnityEvent(); // ouvrir / fermer le cockpit
    [HideInInspector]
    public UnityEvent OnPositionLightChanged = new UnityEvent(); // ouvrir / fermer le cockpit
    [HideInInspector]
    public UnityEvent OnHudLightChanged = new UnityEvent(); // ouvrir / fermer le cockpit
    [HideInInspector]
    public UnityEvent OnPowerSwitchChanged = new UnityEvent(); // alimentation principale on / off
    [HideInInspector]
    public UnityEvent OnGlobalPowerChanged = new UnityEvent(); // alimentation principale on / off
    [HideInInspector]
    public UnityEvent OnLandingLightsChanged = new UnityEvent(); // alimentation principale on / off

    // Liste des composants fournissant de l'energie
    private List<IPowerProvider> powerProviders = new List<IPowerProvider>();

    public static List<PlaneActor> PlaneList = new List<PlaneActor>();

    [HideInInspector]
    public UnityEvent OnDestroyed = new UnityEvent();

    public GameObject explosionObject;

    public GameObject cockpitObject;
    public PlaneTeam planeTeam = PlaneTeam.Blue;

    public void RegisterPowerProvider(IPowerProvider provider)
    {
        if (!powerProviders.Contains(provider))
            powerProviders.Add(provider);
    }

    public bool EnableDebug
    {
        get { return false; }
    }

    // Getter et setter sur l'etat de l'avion. Appelle des events sur lesquels se bind les differents composants
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

    // Calcule l'apport en electricite (en KVA) des differents sous - composants
    public float GetCurrentPower()
    {
        return currentEnginePower;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(massCenter), 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        planePhysic = gameObject.GetComponent<Rigidbody>();
        planePhysic.velocity = transform.forward * initialSpeed;

        // On allume les batteries le temps de parametrer l'etat par defaut de l'avion
        MainPower = true;
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
        OnDestroyed.Invoke();
        PlaneList.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        planePhysic.centerOfMass = massCenter;

        float ro = 1.25f;
        float liftCoef = 1f;
        float surface = 27.87f;

        // Ajoute artificiellement un coefficient de portance a tout l'avion (simuler uniquement la train�e sur l'avion permet d'avoir une physique utilisable pour un gameplay "arcade",
        // cependant il faut en r�alit� aussi prendre en compte l'effet de la portance (qui est beaucoup plus complexe a calculer)
        foreach (var part in GetComponentsInChildren<Rigidbody>())
        {
            float velocity = Mathf.Abs(transform.InverseTransformDirection(part.velocity).z);

            // Calcul classique de portance
            part.AddForce(transform.up * 0.5f * ro * liftCoef * surface * velocity * velocity * Time.deltaTime);
        }

        UpdatePlanePower();
    }

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

    /**
     * Axis and input controll
     */

    [HideInInspector]
    public float ThrustInput = 0;
    [HideInInspector]
    public float PitchInput = 0;
    [HideInInspector]
    public float YawInput = 0;
    [HideInInspector]
    public float RollInput = 0;

    public void SetThrustInput(float input)
    {
        foreach (var thruster in GetComponentsInChildren<Thruster>())
        {
            thruster.setThrustInput(input);
        }

        ThrustInput = input;
    }

    public void SetPitchInput(float input)
    {
        PitchInput = Mathf.Clamp(input, -1, 1);
    }

    public void SetYawInput(float input)
    {
        YawInput = Mathf.Clamp(input, -1, 1);

        foreach (var part in GetComponentsInChildren<WheelCollider>())
            if (part.tag == "Yaw")
                part.steerAngle = Mathf.Pow(input, 3) * 65;
    }

    public void SetRollInput(float input)
    {
        RollInput = Mathf.Clamp(input, -1, 1);
    }
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

    /**
     * Helpers : Informations sur l'etat courant de l'avion
     */
    public float GetSpeedInMetters()
    {
        return transform.InverseTransformDirection(planePhysic.velocity).z;
    }

    public float GetSpeedInNautics()
    {
        return transform.InverseTransformDirection(planePhysic.velocity).z * 1.94384519992989f;
    }

    // = Pitch
    public float GetAttitude()
    {
        return Mathf.Asin(transform.forward.y) / Mathf.PI * 180;
    }

    public float GetRoll()
    {
        return (transform.rotation.eulerAngles.z % 360 + 180 + 360) % 360 - 180;
    }

    /// <summary>
    /// Orientation de l'avion selon l'axe magnetique (en degres)
    /// </summary>
    /// <returns></returns>
    public float GetHeading()
    {
        return transform.rotation.eulerAngles.y;
    }

    public float GetAltitudeInFoots()
    {
        return transform.position.y * 3.281f;
    }


    private void OnCollisionEnter(Collision collision)
    {
        foreach (var component in GetComponentsInChildren<DamageableComponent>())
        {
            Vector3[] points = new Vector3[collision.contactCount];

            for (int i = 0; i < collision.contactCount; ++i)
            {
                points[i] = collision.contacts[i].point;
            }

            component.ApplyDamageAtLocation(points, 0.5f, collision.impulse.magnitude / 200);
        }

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


    void OnExplode()
    {
        planePhysic.isKinematic = true;
        transform.position = transform.position - new Vector3(0, 8, 0);
        if (explosionObject)
        {
            GameObject fxContainer = Instantiate(explosionObject);
            fxContainer.transform.position = transform.position;
            Destroy(fxContainer, 50);
        }
        OnDestroyed.Invoke();
        Destroy(gameObject);
    }

    bool? indoorEnabled = null;
    public void EnableIndoor(bool enable)
    {
        if (indoorEnabled == enable)
            return;

        if (!cockpitObject)
            return;

        cockpitObject.SetActive(enable);
    }

    /**
     * 
     * Thrusters 
     * 
     */


    List<Thruster> thrusterList = new List<Thruster>();
    public void RegisterThruster(Thruster thruster)
    {
        thrusterList.Add(thruster);
    }
    public void UnRegisterThruster(Thruster thruster)
    {
        thrusterList.Remove(thruster);
    }
    public float GetOilPressure()
    {
        return 0;
    }
    public float GetRpmPercent(int thrusterIndex)
    {
        if (thrusterList.Count <= thrusterIndex)
            return 0;
        return thrusterList[thrusterIndex].ThrottlePercent * 0.8f + thrusterList[thrusterIndex].engineStartupPercent * 20f;
    }

    public float GetNozeOpenning()
    {
        return 0;
    }


    /**
     * 
     * Radars 
     * 
     */

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
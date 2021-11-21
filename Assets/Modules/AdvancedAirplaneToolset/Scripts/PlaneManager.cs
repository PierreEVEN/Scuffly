using MLAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

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
 * 
 * @TODO : implementer les batteries
 */

public enum PlaneTeam
{
    Red,
    Blue,
}

[RequireComponent(typeof(Rigidbody))]
public class PlaneManager : NetworkBehaviour
{
    // Definis le centre de gravite physique de l'avion
    public Vector3 massCenter = new Vector3(0, 0, 0);

    // Etat initial de l'avion a sa creation / valeurs par defaut
    public bool initialThrottleNotch = false;
    public bool initialApuSwitch = false;
    public bool initialRetractGear = false;
    public bool initialBrakes = true;
    public float initialSpeed = 0;
    public bool initialPower = false;

    Rigidbody planePhysic;
    private float currentEnginePower;

    // Etat courant de l'avion
    private bool throttleNotch = false;
    private bool apuSwitch = false;
    private bool retractGear = false;
    private bool brakes = true;
    private bool power = false;

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
    public UnityEvent OnPowerSwitchChanged = new UnityEvent(); // alimentation principale on / off
    [HideInInspector]
    public UnityEvent OnGlobalPowerChanged = new UnityEvent(); // alimentation principale on / off

    // Liste des composants fournissant de l'energie
    private List<IPowerProvider> powerProviders = new List<IPowerProvider>();

    public static List<PlaneManager> PlaneList = new List<PlaneManager>();


    public VisualEffectAsset explosionFx;
    public AK.Wwise.Event explosionAudio;

    public GameObject cockpitObject;
    public PlaneTeam planeTeam = PlaneTeam.Blue;

    public void RegisterPowerProvider(IPowerProvider provider)
    {
        if (!powerProviders.Contains(provider))
            powerProviders.Add(provider);
    }

    public bool EnableDebug
    {
        get { return true; }
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
        Brakes = initialBrakes;
        MainPower = initialPower;
    }

    private void OnEnable()
    {
        PlaneList.Add(this);
    }
    private void OnDisable()
    {
        PlaneList.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        planePhysic.centerOfMass = massCenter;

        float ro = 1.25f;
        float liftCoef = 1f;
        float surface = 27.87f;

        // Ajoute artificiellement un coefficient de portance a tout l'avion (simuler uniquement la trainée sur l'avion permet d'avoir une physique utilisable pour un gameplay "arcade",
        // cependant il faut en réalité aussi prendre en compte l'effet de la portance (qui est beaucoup plus complexe a calculer)
        foreach (var part in GetComponentsInChildren<Rigidbody>())
        {
            float velocity = Mathf.Abs(transform.InverseTransformDirection(part.velocity).z);

            // Calcul classique de portance
            part.AddForce(transform.up * 0.5f * ro * liftCoef * surface * velocity * velocity * Time.deltaTime);
            Debug.DrawLine(transform.position, transform.position + transform.up * 0.5f * ro * liftCoef * surface * velocity * velocity * Time.deltaTime);
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
            OnGlobalPowerChanged.Invoke();
            currentEnginePower = newPower;
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
        GUILayout.Label("Brakes : " + Brakes);
        GUILayout.EndArea();
    }

    /**
     * Axis and input controll
     */
    public void SetThrustInput(float input)
    {
        foreach (var thruster in GetComponentsInChildren<Thruster>())
        {
            thruster.setThrustInput(input);
        }

        foreach (var part in GetComponentsInChildren<MobilePart>())
            if (part.tag == "Thrust")
                part.setInput(input);
    }

    public void SetPitchInput(float input)
    {
        input = Mathf.Clamp(input, -1, 1);

        if (!gameObject)
            return;

        foreach (var part in GetComponentsInChildren<MobilePart>())
            if (part.tag == "Pitch")
                part.setInput(input * -1);
    }

    public void SetYawInput(float input)
    {
        input = Mathf.Clamp(input, -1, 1);

        foreach (var part in GetComponentsInChildren<WheelCollider>())
            if (part.tag == "Yaw")
                part.steerAngle = Mathf.Pow(input, 3) * 65;

        foreach (var part in GetComponentsInChildren<MobilePart>())
            if (part.tag == "Yaw")
                part.setInput(input);
    }
    public void SetRollInput(float input)
    {
        input = Mathf.Clamp(input, -1, 1);

        foreach (var part in GetComponentsInChildren<MobilePart>())
            if (part.tag == "Roll")
                part.setInput(input);
    }
    public void Shoot()
    {
        foreach (var part in GetComponentsInChildren<WeaponPod>())
        { // @todo handle weapon selection
            if (part.spawnedWeapon)
            {
                part.Shoot();
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

        Vector3 worldForward = new Vector3(transform.right.x, 0, transform.right.z).normalized;

        float angle = Vector3.SignedAngle(transform.right, worldForward, new Vector3(transform.forward.x, 0, transform.forward.z).normalized);
        return angle;
    }

    // = Yaw
    public float GetHeading()
    {
        return transform.rotation.eulerAngles.y;
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


    public void OnExplode()
    {
        planePhysic.isKinematic = true;
        transform.position = transform.position - new Vector3(0, 8, 0);
        VisualEffect fx = gameObject.AddComponent<VisualEffect>();
        fx.transform.rotation = Quaternion.identity;
        fx.transform.position = transform.position;
        fx.visualEffectAsset = explosionFx;
        fx.Play();
        fx.SetVector3("Position", transform.position);
        explosionAudio.Post(gameObject);
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
}
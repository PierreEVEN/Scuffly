using MLAPI;
using UnityEngine;

/*
 * Class definissant un avion. (a placer a la racine du GameObject correspondant)
 * 
 * Pour fonctionner, l'avion a besoin d'une source d'energie ( ex : batteries qu'il faut d'abord allumer )
 * Le moteur et les systemes hydrauliques ne pourront etre allume que si l'APU est fonctionnel.
 * 
 * 
 * @TODO : implementer les batteries
 */ 

[RequireComponent(typeof(Rigidbody), typeof(NetworkBehaviour))]
public class PlaneManager : MonoBehaviour
{
    // Definis le centre de gravite physique de l'avion
    public Vector3 massCenter = new Vector3(0, 0, 0);

    // Etat initial de l'avion a sa creation / valeurs par defaut
    public bool initialThrottleNotch = false;
    public bool initialApuSwitch = false;
    public bool initialRetractGear = false;
    public bool initialBrakes = true;
    public float initialSpeed = 0;
    public bool initialPower = true;

    Rigidbody planePhysic;

    // Etat courant de l'avion
    private bool throttleNotch = false;
    private bool apuSwitch = false;
    private bool retractGear = false;
    private bool brakes = true;
    private bool power = false;

    // Active ou desactive des fonctionnalites de l'avion

    public bool PowerState
    {
        get { return power; }
        set { 
            power = value;
            if (!power)
            {
                ApuSwitch = false;
            }
        }
    }
    public bool ThrottleNotch
    {
        get { return throttleNotch; }
        set { throttleNotch = value; }
    }
    public bool ApuSwitch
    {
        get { return apuSwitch; }
        set
        {
            if (!power)
                ApuSwitch = false;
            else
                apuSwitch = value;
            foreach (var part in GetComponentsInChildren<APU>())
                if (apuSwitch)
                    part.StartApu();
                else
                    part.StopApu();
        }
    }
    public bool RetractGear
    {
        get { return retractGear; }
        set {
            if (!power)
                return;
            retractGear = value;
            foreach (var part in GetComponentsInChildren<PlaneWheelController>())
                part.Retract(retractGear);
        }
    }
    public bool Brakes
    {
        get { return brakes; }
        set
        {
            if (!power)
                return;
            brakes = value;
            foreach (var part in GetComponentsInChildren<WheelCollider>())
                part.brakeTorque = brakes ? 3000 : 0;
        }
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
        PowerState = true;
        ThrottleNotch = initialThrottleNotch;
        ApuSwitch = initialApuSwitch;
        RetractGear = initialRetractGear;
        Brakes = initialBrakes;
        PowerState = initialPower;
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
        foreach (var part in GetComponentsInChildren<Rigidbody>()) {
            float velocity = Mathf.Abs(transform.InverseTransformDirection(part.velocity).z);

            // Calcul classique de portance
            part.AddForce(transform.up * 0.5f * ro * liftCoef * surface * velocity * velocity * Time.deltaTime);
            Debug.DrawLine(transform.position, transform.position + transform.up * 0.5f * ro * liftCoef * surface * velocity * velocity * Time.deltaTime);
        }
    }

    /**
     * Axis and input controll
     */
    public void SetThrustInput(float input)
    {
        input = Mathf.Clamp(throttleNotch ? input * 0.95f + 0.05f : 0, 0, 1);

        foreach (var thruster in GetComponentsInChildren<Thruster>())
        {
            thruster.set_thrust_input(input);
        }
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
}
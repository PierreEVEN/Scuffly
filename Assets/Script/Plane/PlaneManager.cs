using MLAPI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkBehaviour))]
public class PlaneManager : MonoBehaviour
{
    public Vector3 massCenter = new Vector3(0, 0, 0);
    Rigidbody planePhysic;

    public bool initialThrottleNotch = false;
    public bool initialApuSwitch = false;
    public bool initialRetractGear = false;
    public bool initialBrakes = false;
    public float initialSpeed = 0;

    private bool throttleNotch = false;
    private bool apuSwitch = false;
    private bool retractGear = false;
    private bool brakes = true;

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
            brakes = value;
            foreach (var part in GetComponentsInChildren<WheelCollider>())
                part.brakeTorque = brakes ? 3000 : 0;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        planePhysic = gameObject.GetComponent<Rigidbody>();
        planePhysic.velocity = transform.forward * initialSpeed;

        planePhysic.centerOfMass = massCenter;
        ThrottleNotch = initialThrottleNotch;
        ApuSwitch = initialApuSwitch;
        RetractGear = initialRetractGear;
        Brakes = initialBrakes;
    }

    // Update is called once per frame
    void Update()
    {

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
        {
            if (part.spawnedWeapon)
            {
                part.Shoot();
                return;
            }
        }
    }

    /**
     * Helpers
     */

    public float GetSpeedInMetters()
    {
        return transform.InverseTransformDirection(planePhysic.velocity).z;
    }

    public float GetSpeedInNautics()
    {
        return transform.InverseTransformDirection(planePhysic.velocity).z * 1.94384519992989f;
    }

    public float GetAttitude()
    {
        return Mathf.Asin(transform.forward.y) / Mathf.PI * 180;
    }

    public float GetRoll()
    {
        return (Mathf.Atan2(transform.right.y, transform.right.x) / Mathf.PI * 180 + 360) % 360 - 180;
    }

    public float GetHeading()
    {
        return transform.rotation.eulerAngles.y;
    }
}
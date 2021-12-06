using UnityEngine;

/// <summary>
/// This component controll a retracting gear
/// </summary>
public class PlaneWheelController : PlaneComponent
{
    /// <summary>
    /// The wheel mesh (normally attached to the wheel collider)
    /// </summary>
    public GameObject wheelObject;

    /// <summary>
    /// The component containing the retract animation of the gear
    /// </summary>
    public GameObject animatedObject;

    /// <summary>
    /// The object containing the wheel collider
    /// </summary>
    public GameObject colliderObject;

    /// <summary>
    /// Does this wheel use the yaw axis of the plane to steer
    /// </summary>
    public bool UseSteering = false;

    /// <summary>
    /// Smooth the wheel rotation
    /// </summary>
    public float SteeringRotationSpeed = 100;

    /// <summary>
    /// Does this wheel is brakes
    /// </summary>
    public bool UseBrakes = true;

    private WheelCollider wheelPhysic;
    private float WheelRotation;
    private bool Deployed = true;
    private Animation gearAnim;
    private WheelCollider WheelPhysic
    {
        get
        {
            if (!wheelPhysic && colliderObject)
                wheelPhysic = colliderObject.GetComponent<WheelCollider>();
            if (!wheelPhysic)
                Debug.LogError("failed to get wheel collider");
            return wheelPhysic;
        }
    }
    private Animation AnimationComponent
    {
        get
        {
            if (!gearAnim && animatedObject)
                gearAnim = animatedObject.GetComponent<Animation>();
            if (!gearAnim)
                Debug.LogError("failed to get wheel animation");
            return gearAnim;
        }
    }


    void Start()
    {
        // Alway wake up the wheel, else it will be stuck at startup :/
        WheelPhysic.motorTorque = 0.00001f;
    }

    private void OnEnable()
    {
        Plane.OnBrakesChange.AddListener(UpdateBrakes);
        Plane.OnGearChange.AddListener(UpdateGear);
        Plane.OnGlobalPowerChanged.AddListener(UpdateGear);
        UpdateBrakes();
        UpdateGear();
    }

    private void OnDisable()
    {
        Plane.OnBrakesChange.RemoveListener(UpdateBrakes);
        Plane.OnGearChange.RemoveListener(UpdateGear);
        Plane.OnGlobalPowerChanged.RemoveListener(UpdateGear);
    }

    /// <summary>
    /// The gear require a minimum power to retract.
    /// </summary>
    void UpdateGear()
    {
        if (Plane.GetCurrentPower() > 90)
        {
            if (Plane.RetractGear && Deployed)
                Retract(true);
        }

        if (!Plane.RetractGear && !Deployed)
            Retract(false);
    }

    void UpdateBrakes()
    {
        // Set the brake level of the wheel collider
        WheelPhysic.brakeTorque = UseBrakes && (Plane.ParkingBrakes || Plane.Brakes) ? 3000 : 0;
    }

    void Update()
    {
        // Make the rotation of the mesh match the physics
        if (UseSteering)
        {
            // Set the angular rotation of the wheel collider
            float desiredRotation = Plane.RetractGear ? 0 : Plane.YawInput * 70;
            WheelPhysic.steerAngle = WheelPhysic.steerAngle + Mathf.Clamp(desiredRotation - WheelPhysic.steerAngle, -Time.deltaTime * SteeringRotationSpeed, Time.deltaTime * SteeringRotationSpeed);
        }
        float rps = WheelPhysic.rpm / 60.0f;
        WheelRotation = (WheelRotation + rps * 360 * Time.deltaTime) % 360.0f;
        wheelObject.transform.localRotation = Quaternion.Euler(WheelRotation, WheelPhysic.steerAngle, 0);
    }

    /// <summary>
    /// Gear animation
    /// </summary>
    /// <param name="retract"></param>
    void Retract(bool retract)
    {
        if (retract != Deployed) return;

        if (!AnimationComponent.clip)
            Debug.LogError("missing wheel animation clip for " + name);

        if (Deployed)
        {
            AnimationComponent[AnimationComponent.clip.name].speed = 1;
            AnimationComponent[AnimationComponent.clip.name].time = Mathf.Max(AnimationComponent[AnimationComponent.clip.name].time, 0);
            AnimationComponent.Play(AnimationComponent.clip.name);
            Deployed = false;
        }
        else
        {
            // To retract the gear, we play the same animation, but in reversed speed
            AnimationComponent[AnimationComponent.clip.name].speed = -1;
            if (AnimationComponent[AnimationComponent.clip.name].time < 0.01)
                AnimationComponent[AnimationComponent.clip.name].time = AnimationComponent.clip.length - Time.deltaTime;
            AnimationComponent.Play(AnimationComponent.clip.name);
            Deployed = true;
        }
    }
}

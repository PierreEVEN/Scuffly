using UnityEngine;

/**
 *  @Author : Pierre EVEN
 *  
 *  Fait tourner les roues selons l'etat actuel de leur component physique / gere les animations de sortie / rentree des trains
 *  Le skeletal mesh doit contenir un bone contenant le mot cle "axis". Ce bone sert à determiner la position de la roue.
 */
public class PlaneWheelController : PlaneComponent
{
    //@TODO : improve the wheel system to be more reliable

    WheelCollider wheelPhysic;
    float WheelRotation;
    bool Deployed = true;
    Animation gearAnim;

    public GameObject wheelObject;
    public GameObject animatedObject;
    public GameObject colliderObject;
    public bool UseSteering = false;
    public float SteeringRotationSpeed = 100;

    // Parametre l'utilisation des freins sur cette roue
    public bool UseBrakes = true;

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


    // Start is called before the first frame update
    void Start()
    {
        WheelPhysic.motorTorque = 0.00001f; // Evite de bloquer les roues
    }

    private void OnEnable()
    {
        Plane.OnBrakesChange.AddListener(UpdateBrakes);
        Plane.OnGearChange.AddListener(UpdateGear);
        UpdateBrakes();
        UpdateGear();
    }

    private void OnDisable()
    {
        Plane.OnBrakesChange.RemoveListener(UpdateBrakes);
        Plane.OnGearChange.RemoveListener(UpdateGear);
    }

    // Appelé quand un changement d'etat est detecté. La rentree du train necessite un minimum d'energie
    void UpdateGear()
    {
        // On ne peut pas rentrer le train si pas assez d'energie
        if (Plane.GetCurrentPower() > 90)
        {
            if (Plane.RetractGear && Deployed)
                Retract(true);
        }

        if (!Plane.RetractGear && !Deployed)
            Retract(false);
    }

    // Mise a jour de l'etat des freins
    void UpdateBrakes()
    {
        WheelPhysic.brakeTorque = UseBrakes && Plane.Brakes ? 3000 : 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (UseSteering)
        {
            float desiredRotation = Plane.RetractGear ? 0 : Plane.YawInput * 70;
            WheelPhysic.steerAngle = WheelPhysic.steerAngle + Mathf.Clamp(desiredRotation - WheelPhysic.steerAngle, -Time.deltaTime * SteeringRotationSpeed, Time.deltaTime * SteeringRotationSpeed);
        }
        float rps = WheelPhysic.rpm / 60.0f;
        WheelRotation = (WheelRotation + rps * 360 * Time.deltaTime) % 360.0f;
        wheelObject.transform.localRotation = Quaternion.Euler(WheelRotation, WheelPhysic.steerAngle, 0);
    }

    // Rentre ou sort le train d'aterissage
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
            // Pour rentrer le train : on joue l'animation dans le sens inverse en partant de la fin
            AnimationComponent[AnimationComponent.clip.name].speed = -1;
            if (AnimationComponent[AnimationComponent.clip.name].time < 0.01)
                AnimationComponent[AnimationComponent.clip.name].time = AnimationComponent.clip.length - Time.deltaTime;
            AnimationComponent.Play(AnimationComponent.clip.name);
            Deployed = true;
        }
    }
}

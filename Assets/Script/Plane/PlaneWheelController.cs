using UnityEngine;

/**
 *  @Author : Pierre EVEN
 *  
 *  Fait tourner les roues selons l'etat actuel de leur component physique / gere les animations de sortie / rentree des trains
 *  Le skeletal mesh doit contenir un bone contenant le mot cle "axis". Ce bone sert à determiner la position de la roue.
 */
public class PlaneWheelController : PlaneComponent
{
    WheelCollider wheelPhysic;
    private Transform wheelAxisBone;
    float WheelRotation;
    Animation gearAnim;
    bool Deployed = true;

    // Parametre l'utilisation des freins sur cette roue
    public bool UseBrakes = true;

    private WheelCollider WheelPhysic
    {
        get
        {
            if (!wheelPhysic)
                wheelPhysic = gameObject.GetComponentInParent<WheelCollider>();
            if (!wheelPhysic)
                Debug.LogError("failed to get wheel collider");
            return wheelPhysic;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        WheelPhysic.motorTorque = 0.001f; // Evite de bloquer les roues
        if (!WheelPhysic)
            Debug.LogError("failed to find wheel collider");
        gearAnim = GetComponentInParent<Animation>();
        var skeletalMesh = GetComponentInParent<SkinnedMeshRenderer>();
        if (skeletalMesh)
        {
            // Cherche parmis tous les bones du squelette un bone nommé "*axis*". Le bone sur lequel sera attaché la roue sera le point de fin de ce bone.
            foreach (var bone in skeletalMesh.bones)
            {
                if (bone.name.Contains("_end") && bone.parent.name.ToLower().Contains("axis"))
                {
                    wheelAxisBone = bone;
                }
            }
        }
        else
        {
            Debug.LogError("failed to get wheel skeleton");
        }

        if (wheelAxisBone)
            gameObject.transform.parent = wheelAxisBone;

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

    void UpdateBrakes()
    {
        WheelPhysic.brakeTorque = UseBrakes && Plane.Brakes ? 3000 : 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (!wheelAxisBone)
            return;
        WheelRotation = (WheelRotation + WheelPhysic.rpm * 6 * Time.deltaTime) % 360.0f;
        gameObject.transform.rotation = transform.parent.rotation * Quaternion.FromToRotation(wheelAxisBone.position, wheelAxisBone.parent.position) * Quaternion.Euler(0, 0, WheelPhysic.steerAngle) * Quaternion.Euler(0, -WheelRotation, 90);
    }

    public void Retract(bool retract)
    {
        if (!gearAnim || retract != Deployed) return;
        if (Deployed)
        {
            gearAnim[gearAnim.clip.name].speed = 1;
            gearAnim[gearAnim.clip.name].time = Mathf.Max(gearAnim[gearAnim.clip.name].time, 0);
            gearAnim.Play(gearAnim.clip.name);
            Deployed = false;
        }
        else
        {
            // Pour rentrer le train : on joue l'animation dans le sens inverse en partant de la fin
            gearAnim[gearAnim.clip.name].speed = -1;
            if (gearAnim[gearAnim.clip.name].time < 0.01)
                gearAnim[gearAnim.clip.name].time = gearAnim.clip.length - Time.deltaTime;
            gearAnim.Play(gearAnim.clip.name);
            Deployed = true;
        }
    }
}

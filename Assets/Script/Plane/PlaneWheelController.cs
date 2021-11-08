using UnityEngine;

/**
 *  @Author : Pierre EVEN
 *  
 *  Fait tourner les roues selons l'etat actuel de leur component physique / gere les animations de sortie / rentree des trains
 *  Le skeletal mesh doit contenir un bone contenant le mot cle "axis". Ce bone sert à determiner la position de la roue.
 */
public class PlaneWheelController : MonoBehaviour
{
    WheelCollider WheelPhysic;
    private Transform wheelAxisBone;
    float WheelRotation;
    Animation gearAnim;
    bool Deployed = true;

    // Start is called before the first frame update
    void Start()
    {
        WheelPhysic = gameObject.GetComponentInParent<WheelCollider>();
        WheelPhysic.brakeTorque = 0.0f;
        WheelPhysic.motorTorque = 0.01f;

        gearAnim = GetComponentInParent<Animation>();
        var skeletalMesh = GetComponentInParent<SkinnedMeshRenderer>();

        // Cherche parmis tous les bones du squelette un bone nommé "*axis*". Le bone sur lequel sera attaché la roue sera le point de fin de ce bone.
        foreach (var bone in skeletalMesh.bones)
        {
            if (bone.name.Contains("_end") && bone.parent.name.ToLower().Contains("axis"))
            {
                wheelAxisBone = bone;
            }
        }

        if (wheelAxisBone)
            gameObject.transform.parent = wheelAxisBone;
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

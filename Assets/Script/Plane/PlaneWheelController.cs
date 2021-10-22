using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *  @Author : Pierre EVEN
 */
public class PlaneWheelController : MonoBehaviour
{
    WheelCollider WheelPhysic;
    float WheelRotation = 0;
    private Transform wheelAxisBone;
    Animation gearAnim;
    bool Deployed = true;

    // Start is called before the first frame update
    void Start()
    {
        WheelPhysic = gameObject.GetComponent<WheelCollider>();
        WheelPhysic.brakeTorque = 0.0f;

        gearAnim = GetComponentInParent<Animation>();
        var skeletalMesh = GetComponentInParent<SkinnedMeshRenderer>();


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

    public void Switch()
    {
        if (!gearAnim) return;
        if (Deployed)
        {
            gearAnim[gearAnim.clip.name].speed = 1;
            gearAnim[gearAnim.clip.name].time = Mathf.Max(gearAnim[gearAnim.clip.name].time, 0);
            gearAnim.Play(gearAnim.clip.name);
            Deployed = false;
        }
        else
        {
            gearAnim[gearAnim.clip.name].speed = -1;
            if (gearAnim[gearAnim.clip.name].time < 0.01)
            gearAnim[gearAnim.clip.name].time = gearAnim.clip.length - Time.deltaTime;
            gearAnim.Play(gearAnim.clip.name);
            Deployed = true;
        }
    }
}

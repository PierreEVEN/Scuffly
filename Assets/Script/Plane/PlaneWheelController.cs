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
    public int BoneToAttach = 0;
    Animation gearAnim;
    bool Deployed = true;

    // Start is called before the first frame update
    void Start()
    {
        WheelPhysic = gameObject.GetComponent<WheelCollider>();
        WheelPhysic.brakeTorque = 0.0f;

        gearAnim = GetComponentInParent<Animation>();
        var skeletalMesh = GetComponentInParent<SkinnedMeshRenderer>();
        gameObject.transform.parent = skeletalMesh.bones[BoneToAttach];
    }

    // Update is called once per frame
    void Update()
    {
        WheelRotation = (WheelRotation + WheelPhysic.rpm * 6 * Time.deltaTime) % 360.0f;
        gameObject.transform.rotation = gameObject.transform.parent.rotation * Quaternion.Euler(WheelRotation, -90, 0);
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

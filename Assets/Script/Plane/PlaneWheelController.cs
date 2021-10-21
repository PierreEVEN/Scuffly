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
    Animation animator;
    bool Deployed = true;
    public AnimationClip animation;

    // Start is called before the first frame update
    void Start()
    {
        WheelPhysic = gameObject.GetComponent<WheelCollider>();
        WheelPhysic.brakeTorque = 0.0f;

        animator = GetComponentInParent<Animation>();

        var skeletalMesh = GetComponentInParent<SkinnedMeshRenderer>();
        gameObject.transform.parent = skeletalMesh.bones[BoneToAttach];
        animator.AddClip(animation, "extract");
    }

    // Update is called once per frame
    void Update()
    {
        WheelRotation = (WheelRotation + WheelPhysic.rpm * 6 * Time.deltaTime) % 360.0f;
        gameObject.transform.rotation = gameObject.transform.parent.rotation * Quaternion.Euler(WheelRotation, -90, 0);
    }

    public void Switch()
    {
        if (!animator) return;
        if (Deployed)
        {
            animator.Play();
            Deployed = false;
        }
        else
        {
            animator.Rewind();
            animator.Play();
            animator.Stop();
            Deployed = true;
        }
    }
}

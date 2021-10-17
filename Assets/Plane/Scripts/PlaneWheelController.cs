using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWheelController : MonoBehaviour
{
    WheelCollider WheelPhysic;
    float WheelRotation = 0;
    public int BoneToAttach = 0;
    // Start is called before the first frame update
    void Start()
    {
        WheelPhysic = gameObject.GetComponent<WheelCollider>();
        WheelPhysic.brakeTorque = 0.0f;

        var skeletalMesh = GetComponentInParent<SkinnedMeshRenderer>();
        gameObject.transform.parent = skeletalMesh.bones[BoneToAttach];
    }

    // Update is called once per frame
    void Update()
    {
        WheelRotation = (WheelRotation + WheelPhysic.rpm * 6 * Time.deltaTime) % 360.0f;
        gameObject.transform.rotation = Quaternion.Euler(WheelRotation, 0, 0);
    }
}

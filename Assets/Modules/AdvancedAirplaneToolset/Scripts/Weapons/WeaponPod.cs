using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A weapon pod is placed under the wing, and is used to attach missiles to the aircraft
/// </summary>
public class WeaponPod : PlaneComponent
{
    /// <summary>
    /// A list of weapons that can be spawned from the current pod
    /// </summary>
    public List<GameObject> spawnableWeapons = new List<GameObject>();

    /// <summary>
    /// The weapon that have been spawned
    /// </summary>
    [HideInInspector]
    public PodItem attachedPodItem;

    void Start()
    {
        if (spawnableWeapons.Count == 0) return;

        // 1. We spawn a weapon from the list of available weapons
        GameObject podItemObject = Instantiate(spawnableWeapons[0]);
        podItemObject.transform.parent = transform;
        podItemObject.transform.position = transform.position;
        podItemObject.transform.rotation = transform.rotation;
        attachedPodItem = podItemObject.GetComponent<PodItem>();
        if (!attachedPodItem)
            Destroy(podItemObject);
    }

    /// <summary>
    /// Shoot the weapon attached to the pod (missile / bomb / detach fuel tank ...)
    /// </summary>
    /// <param name="target"></param>
    public void Shoot(GameObject target)
    {
        // we check there is an attached weapon first
        if (!attachedPodItem)
            return;
        PodItem comp = attachedPodItem.GetComponent<PodItem>();
        if (comp)
            comp.Shoot(Plane.gameObject, GetComponentInParent<Rigidbody>().velocity - transform.up * 10, target);
        attachedPodItem = null; // Mark the pod as empty
    }
}

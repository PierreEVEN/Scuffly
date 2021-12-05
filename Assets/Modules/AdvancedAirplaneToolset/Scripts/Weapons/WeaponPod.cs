using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Point d'attache d'armements. Se place sous chaque aile.
 */
public class WeaponPod : PlaneComponent
{
    public List<GameObject> spawnableWeapons = new List<GameObject>();

    [HideInInspector]
    public PodItem attachedPodItem;

    void Start()
    {
        // Spawn une arme random parmis la liste (0 pour l'instant)
        if (spawnableWeapons.Count == 0) return;

        GameObject podItemObject = Instantiate(spawnableWeapons[0]);
        podItemObject.transform.parent = transform;
        podItemObject.transform.position = transform.position;
        podItemObject.transform.rotation = transform.rotation;
        attachedPodItem = podItemObject.GetComponent<PodItem>();
        if (!attachedPodItem)
            Destroy(podItemObject);
    }

    // Tire l'arme attach�e au pod
    public void Shoot(GameObject target)
    {
        // On regarde si un arme est attachee au pod, si c'est le cas on l'active.
        if (!attachedPodItem)
            return;
        PodItem comp = attachedPodItem.GetComponent<PodItem>();
        if (comp)
            comp.Shoot(Plane.gameObject, GetComponentInParent<Rigidbody>().velocity - transform.up * 10, target);
        attachedPodItem = null; // l'arme a ete tiree
    }
}

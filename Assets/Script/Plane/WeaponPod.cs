using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Point d'attache d'armements. Se place sous chaque aile.
 */
public class WeaponPod : MonoBehaviour
{
    public List<GameObject> spawnableWeapons = new List<GameObject>();

    [HideInInspector]
    public GameObject spawnedWeapon;

    // Start is called before the first frame update
    void Start()
    {
        // Spawn une arme random parmis la liste (0 pour l'instant)
        if (spawnableWeapons.Count == 0) return;

        spawnedWeapon = GameObject.Instantiate(spawnableWeapons[0]);
        spawnedWeapon.transform.parent = transform;
        spawnedWeapon.transform.position = transform.position;
        spawnedWeapon.transform.rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        // On regarde si un arme est attachee au pod, si c'est le cas on l'active.
        if (!spawnedWeapon)
            return;
        Rocket comp = spawnedWeapon.GetComponent<Rocket>();
        if (comp)
            comp.Shoot(GetComponentInParent<Rigidbody>().velocity, GameObject.Find("Windmill"));
        spawnedWeapon = null; // l'arme a ete tiree
    }
}

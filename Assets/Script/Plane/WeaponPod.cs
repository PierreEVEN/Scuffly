using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPod : MonoBehaviour
{
    public List<GameObject> spawnableWeapons = new List<GameObject>();

    [HideInInspector]
    public GameObject spawnedWeapon;

    // Start is called before the first frame update
    void Start()
    {
        return;
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
        if (!spawnedWeapon)
            return;
        Debug.Log("shoot weapon");
        Rocket comp = spawnedWeapon.GetComponent<Rocket>();
        if (comp)
            comp.Shoot(GetComponentInParent<Rigidbody>().velocity);
        spawnedWeapon = null;
    }
}

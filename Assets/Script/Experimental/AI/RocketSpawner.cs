using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{

    public float SpawnDelay = 10;

    public float initialDelay = 5;

    public GameObject rocketToSpawn;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        initialDelay -= Time.deltaTime;
        if (initialDelay <= 0)
        {
            initialDelay = SpawnDelay;
            if (rocketToSpawn)
            {
                GameObject target = GameObject.FindWithTag("Plane");
                if (!target)
                    return;
                if (Vector3.Distance(target.transform.position, transform.position) > 3000)
                    return;

                GameObject rocketObj = GameObject.Instantiate(rocketToSpawn);

                rocketObj.transform.position = transform.position;
                rocketObj.transform.rotation = transform.rotation;
                Rocket rocket = rocketObj.GetComponentInChildren<Rocket>();
                rocket.Shoot(transform.forward * 1, target);
            }
        }
    }
}

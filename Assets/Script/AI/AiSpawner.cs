using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSpawner : MonoBehaviour
{
    public GameObject plane;
    // Start is called before the first frame update
    void Start()
    {
        GameObject newPlane = Instantiate(plane);

        newPlane.AddComponent<PlaneAIController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

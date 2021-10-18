using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkBehaviour))]
public class PlaneManager : MonoBehaviour
{
    public Vector3 massCenter = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().centerOfMass = massCenter;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

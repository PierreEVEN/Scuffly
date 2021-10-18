using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaneSpawnpoint : MonoBehaviour
{

    public bool InFlight = false;
    public bool FlightReady = false;
    public Vector3 InitialVelocity = new Vector3();

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(gameObject.transform.position, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

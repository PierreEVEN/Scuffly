using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVelocityVectorManager : MonoBehaviour
{
    private PlaneATHManager athManager;


    // Start is called before the first frame update
    private void OnEnable()
    {
        athManager = GetComponentInParent<PlaneATHManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = athManager.WorldDirectionToScreenPosition(athManager.owningPlane.GetComponent<Rigidbody>().velocity.normalized);
    }
}

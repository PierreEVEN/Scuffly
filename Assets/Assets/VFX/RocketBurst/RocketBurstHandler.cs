using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

// Permet de mettre a jour la position de spawn de la particule dans le monde
[RequireComponent(typeof(VisualEffect)), ExecuteInEditMode]
public class RocketBurstHandler : MonoBehaviour
{
    VisualEffect vfx;
    // Start is called before the first frame update
    void OnEnable()
    {
        vfx = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = gameObject.GetComponentInParent<Rigidbody>();

        if (vfx.HasVector3("EmitterPosition"))
            vfx.SetVector3("EmitterPosition", transform.position);
        if (rb && vfx.HasVector3("EmitterVelocity"))
            vfx.SetVector3("EmitterVelocity", rb.velocity);
    }
}

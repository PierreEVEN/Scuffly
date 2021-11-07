using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAtitudeManager : MonoBehaviour
{
    PlaneATHManager athManager;
    // Start is called before the first frame update
    void Start()
    {
        athManager = GetComponentInParent<PlaneATHManager>();
        if (!athManager)
            Debug.LogError("missing arhManager in parent hierarchy");
    }

    // Update is called once per frame
    void Update()
    {
        float roll = athManager.owningPlane.GetRoll();
        float rollRad = roll / 180 * Mathf.PI;
        float attitude = Mathf.Tan(athManager.owningPlane.GetAttitude() / 180 * Mathf.PI) * -1000f * (athManager.owningPlane.transform.up.y <= 0 ? -1 : 1);

        gameObject.transform.rotation = Quaternion.Euler(0, 0, athManager.owningPlane.GetRoll());
        gameObject.transform.localPosition = new Vector3(Mathf.Sin(-rollRad) * attitude, Mathf.Cos(-rollRad) * attitude, 0);
    }
}

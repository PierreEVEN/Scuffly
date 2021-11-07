using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIVelocityManager : MonoBehaviour
{
    Text text;
    PlaneATHManager athManager;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        athManager = GetComponentInParent<PlaneATHManager>();
        if (!athManager)
            Debug.LogError("missing arhManager in parent hierarchy");
    }

    // Update is called once per frame
    void Update()
    {
        text.text = string.Format("{0}", (int)athManager.owningPlane.GetSpeedInNautics());
    }
}

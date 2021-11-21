using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ATHUiRenderer : MonoBehaviour
{
    public GameObject RendererdUI;
    GameObject instanciedUI;

    private void OnEnable()
    {
        if (!instanciedUI && RendererdUI)
        {
            instanciedUI = GameObject.Instantiate(RendererdUI);
            instanciedUI.hideFlags = HideFlags.DontSave;
            instanciedUI.GetComponent<PlaneATHManager>().owningPlane = GetComponentInParent<PlaneManager>();
            if (!instanciedUI.GetComponent<PlaneATHManager>().owningPlane)
                Debug.LogError("missing plane in parent hierarchy");
        }
    }

    private void OnDisable()
    {
        if (instanciedUI)
            DestroyImmediate(instanciedUI);
    }
}

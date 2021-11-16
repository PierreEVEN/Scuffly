using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneATHManager : MonoBehaviour
{
    public PlaneManager owningPlane;
    Canvas canvas;
    CanvasGroup canvasGroup;
    float alpha = 0;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!owningPlane)
            return;
        if (!canvas)
        {
            canvas = GetComponentInChildren<Canvas>();
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            if (!canvas)
                return;
        }

        canvas.enabled = owningPlane.PowerState;
        alpha = Mathf.Clamp01(alpha + (owningPlane.PowerState ? 0.5f * Time.deltaTime : -4f * Time.deltaTime));
        canvasGroup.alpha = alpha;
    }
}

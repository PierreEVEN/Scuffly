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

        canvas.enabled = owningPlane.MainPower;
        alpha = Mathf.Clamp01(alpha + (owningPlane.MainPower ? 0.5f * Time.deltaTime : -4f * Time.deltaTime));
        canvasGroup.alpha = Mathf.Clamp01((owningPlane.GetCurrentPower() - 80) / 30);
    }
}

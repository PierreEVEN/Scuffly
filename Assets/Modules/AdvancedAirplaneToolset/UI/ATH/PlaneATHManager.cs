using UnityEngine;

public class PlaneATHManager : MonoBehaviour
{
    public PlaneManager owningPlane;
    Canvas canvas;
    CanvasGroup canvasGroup;
    float alpha = 0;

    public float AthScale = 0.39f;

    void OnEnable()
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

    public Vector2 WorldDirectionToScreenPosition(Vector3 worldDirection)
    {
        Vector3 PlaneRelativeDirection = owningPlane.transform.InverseTransformDirection(worldDirection).normalized;
        var containerTransform = canvas.GetComponent<RectTransform>();
        return new Vector2(PlaneRelativeDirection.x * containerTransform.sizeDelta.x, PlaneRelativeDirection.y * containerTransform.sizeDelta.y) / 0.39f;
    }
}
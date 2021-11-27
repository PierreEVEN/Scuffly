using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class ThreatWarningIndicator : PlaneComponent
{
    Dictionary<GameObject, GameObject> displayedTargets = new Dictionary<GameObject, GameObject>();

    Canvas container;
    private void OnEnable()
    {
        Plane.GetRadar().OnDetectNewTarget.AddListener(AddTarget);
        Plane.GetRadar().OnLostTarget.AddListener(RemoveTarget);

        container = GetComponent<Canvas>();
        foreach (var target in Plane.GetRadar().scannedTargets)
            AddTarget(target.Key);
    }

    private void OnDisable()
    {
        Plane.GetRadar().OnDetectNewTarget.RemoveListener(AddTarget);
        Plane.GetRadar().OnLostTarget.RemoveListener(RemoveTarget);

        foreach (var target in displayedTargets)
            Destroy(target.Value);
        displayedTargets.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var target in Plane.GetRadar().scannedTargets)
        {
            Vector3 relativePosition = (target.Value.ScannedWorldPosition - Plane.transform.position);
            relativePosition = Plane.transform.InverseTransformDirection(relativePosition.normalized);
            float distance = (target.Value.ScannedWorldPosition - Plane.transform.position).magnitude;
            Vector3 relativeDirection = relativePosition.normalized;
            GameObject icon = displayedTargets[target.Key];
            icon.transform.localPosition = new Vector3(relativeDirection.x, relativeDirection.z, 0).normalized * Mathf.Min(50, distance / 100);
        }
    }

    void AddTarget(GameObject newTarget)
    {
        GameObject icon = new GameObject("target_" + newTarget.name);
        icon.transform.parent = container.transform;
        icon.transform.localRotation = Quaternion.identity;
        icon.transform.localScale = new Vector3(1, 1, 1);
        icon.transform.localPosition = new Vector3(0, 0, 0);
        RawImage image = icon.AddComponent<RawImage>();
        image.color = Color.red;
        icon.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
        displayedTargets.Add(newTarget, icon);
    }
    void RemoveTarget(GameObject removedTarget)
    {
        if (displayedTargets.ContainsKey(removedTarget))
        {
            Destroy(displayedTargets[removedTarget]);
            displayedTargets.Remove(removedTarget);
        }
    }
}

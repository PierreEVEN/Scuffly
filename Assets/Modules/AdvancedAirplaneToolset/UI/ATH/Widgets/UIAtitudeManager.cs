using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAtitudeManager : MonoBehaviour
{
    PlaneATHManager athManager;
    Canvas container;
    // Start is called before the first frame update
    void OnEnable()
    {
        athManager = GetComponentInParent<PlaneATHManager>();
        if (!athManager)
            Debug.LogError("missing arhManager in parent hierarchy");
        container = GetComponentInParent<Canvas>();

    }

    bool init = false;

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            init = true;
            for (int i = -90; i <= 90; i += 5)
            {
                AddGraduation(i);
            }
        }

        Vector3 screenPos = athManager.WorldDirectionToScreenPosition(new Vector3(athManager.owningPlane.transform.forward.x, 0, athManager.owningPlane.transform.forward.z).normalized);

        float roll = athManager.owningPlane.GetRoll();
        float rollRad = roll / 180 * Mathf.PI;

        gameObject.transform.rotation = Quaternion.Euler(0, 0, athManager.owningPlane.GetRoll());
        gameObject.transform.localPosition = screenPos;// new Vector3(Mathf.Sin(-rollRad) * screenPos.y, Mathf.Cos(-rollRad) * screenPos.y, 0);
    }

    struct Graduation
    {
        float offset;
        GameObject container;
    }

    List<Graduation> OnScreenGraduations = new List<Graduation>();

    void UpdateGraduations()
    {
        //athManager.owningPlane.GetAttitude()
    }

    void AddGraduation(float angle)
    {
        Vector2 offset = athManager.WorldDirectionToScreenPosition(Quaternion.AngleAxis(angle, new Vector3(athManager.owningPlane.transform.right.x, 0, athManager.owningPlane.transform.right.z)) * new Vector3(athManager.owningPlane.transform.forward.x, 0, athManager.owningPlane.transform.forward.z));

        GameObject newGrad = new GameObject("grad_" + angle);
        newGrad.transform.parent = transform;
        newGrad.transform.localPosition = offset;
        newGrad.transform.localRotation = Quaternion.identity;
        newGrad.transform.localScale = Vector3.one;
        RawImage im = newGrad.AddComponent<RawImage>();
        newGrad.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 2);

    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawn graduation and move it on the HUD to tell the current inclination of the aircraft
/// //@DEPRECATED
/// </summary>
public class UIAtitudeManager : HUDComponent
{
    bool init = false;

    void Update()
    {
        if (!init)
        {
            init = true;
            for (int i = -90; i <= 90; i += 5)
            {
                // Only first time, add the graduation
                // @todo : find why it doesn't works from Start functions
                AddGraduation(i);
            }
        }

        // Move all the graduations depending on the roll and the pitch of the aircraft
        Vector3 screenPos = HUD.WorldDirectionToScreenPosition(new Vector3(Plane.transform.forward.x, 0, Plane.transform.forward.z).normalized);
        float roll = Plane.GetRoll();
        float rollRad = roll / 180 * Mathf.PI;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, Plane.GetRoll());
        gameObject.transform.localPosition = screenPos;
    }

    void AddGraduation(float angle)
    {
        Vector2 offset = HUD.WorldDirectionToScreenPosition(Quaternion.AngleAxis(angle, new Vector3(Plane.transform.right.x, 0, Plane.transform.right.z)) * new Vector3(Plane.transform.forward.x, 0, Plane.transform.forward.z));

        GameObject newGrad = new GameObject("grad_" + angle);
        newGrad.transform.parent = transform;
        newGrad.transform.localPosition = offset;
        newGrad.transform.localRotation = Quaternion.identity;
        newGrad.transform.localScale = Vector3.one;
        RawImage im = newGrad.AddComponent<RawImage>();
        newGrad.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 2);

    }
}

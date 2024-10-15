using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle the HUD and it's different components //@TODO : improve HUDManager
/// </summary>
public class HUDManager : MonoBehaviour
{
    /// <summary>
    /// Rendering scale (must match the material parameter)
    /// </summary>
    public float AthScale = 0.39f;

    /// <summary>
    /// The forward vector icon
    /// </summary>
    public GameObject forwardVectorContainer;

    /// <summary>
    /// The icon showing velocity of the plane
    /// </summary>
    public GameObject velocityVectorContainer;

    /// <summary>
    /// The gameObject containing a scale to show the current velocity
    /// </summary>
    public GameObject VelocityScale;
    public GameObject velocityText;

    /// <summary>
    /// The gameObject containing a scale to show the current altitude
    /// </summary>
    public GameObject AltitudeScale;
    public GameObject AltitudeText;

    /// <summary>
    /// The different displayed widget on screen
    /// </summary>
    public GameObject irMissileWidget;
    public GameObject radarMissileWidget;
    public GameObject attitudeWidget;

    public GameObject HeadingScale;
    public GameObject HeadingText;

    GameObject currentDisplayedWidget = null;
    PlaneActor owningPlane;
    Canvas canvas;
    CanvasGroup canvasGroup;
    float alpha = 0;

    public PlaneActor Plane
    {
        get
        {
            // If plane is still not set, try to retrieve it in the hierarchy
            if (!owningPlane)
            {
                owningPlane = GetComponentInParent<PlaneActor>();
                if (!owningPlane)
                    Debug.LogError("failed to find plane in parent hierarchy");
            }
            return owningPlane;
        }
        set
        {
            owningPlane = value;
        }
    }

    void OnEnable()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();

        // Hide all the widgets. We will reenable them later
        if (irMissileWidget)
            irMissileWidget.SetActive(false);
        if (radarMissileWidget)
            radarMissileWidget.SetActive(false);
    }

    void Update()
    {
        if (!Plane)
            return;
        if (!canvas)
        {
            canvas = GetComponentInChildren<Canvas>();
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            if (!canvas)
                return;
        }

        // Update the intensity of the HUD (depending on the brightness button and the plane power)
        canvas.enabled = Plane.MainPower;
        alpha = Mathf.Clamp01(alpha + (Plane.MainPower ? 0.5f * Time.deltaTime : -4f * Time.deltaTime));
        canvasGroup.alpha = Mathf.Min(Plane.HudLightLevel, Mathf.Clamp01((Plane.GetCurrentPower() - 80) / 30));

        // Update informations about velocity and forward vector on the HUD (move the widgets)
        if (Plane.MainPower)
        {
            if (forwardVectorContainer)
            {
                Vector2 Forward = WorldDirectionToScreenPosition(Plane.transform.forward);
                forwardVectorContainer.transform.localPosition = new Vector3(Forward.x, Forward.y, 0);
            }

            if (velocityVectorContainer)
            {
                Vector2 Forward = WorldDirectionToScreenPosition(Plane.GetComponent<Rigidbody>().velocity);
                velocityVectorContainer.transform.localPosition = new Vector3(Forward.x, Forward.y, 0);
            }
        }

        if (velocityText)
        {
            Text velocityTextComp = velocityText.GetComponent<Text>();
            if (velocityTextComp)
                velocityTextComp.text = ((int)Plane.GetSpeedInNautics()).ToString();
        }

        if (VelocityScale)
        {
            VelocityScale.SetActive(!Plane.WeaponSystem.IsEnabled || !Plane.RetractGear);
            VelocityScale.transform.localPosition = new Vector3(0, -Plane.GetSpeedInNautics() * 1f + 500, 0);
        }

        if (AltitudeText)
        {
            Text altitudeTextComp = AltitudeText.GetComponent<Text>();
            if (altitudeTextComp)
                altitudeTextComp.text = (((int)Plane.GetAltitudeInFoots()) / 1000.0f).ToString();
        }

        if (AltitudeScale)
        {
            AltitudeScale.SetActive(!Plane.WeaponSystem.IsEnabled || !Plane.RetractGear);
            AltitudeScale.transform.localPosition = new Vector3(0, -Plane.GetAltitudeInFoots() * 0.1f + 2500, 0);
        }

        if (HeadingText)
        {
            Text headingtextComp = HeadingText.GetComponent<Text>();
            if (headingtextComp)
                headingtextComp.text = ((int)Plane.GetHeading()).ToString();
        }

        if (HeadingScale)
        {
            HeadingScale.SetActive(!Plane.WeaponSystem.IsEnabled || !Plane.RetractGear);
            HeadingScale.transform.localPosition = new Vector3(-Plane.GetHeading() * 3.8f + 682, 0, 0);
        }

        UpdateDisplayedWidget();

        if (attitudeWidget)
        {
            // Turn the attitude widget so it alway head to the Y world axis 
            attitudeWidget.transform.localRotation = Quaternion.Euler(0, 0, Plane.GetRoll() * -1);
            float offset = Mathf.Asin(Plane.transform.forward.y) / (Mathf.PI / 2) * -2700;
            float rot = Plane.GetRoll() / -180 * Mathf.PI;
            attitudeWidget.transform.localPosition = new Vector2(-Mathf.Sin(rot) * offset, Mathf.Cos(rot) * offset);
        }

    }

    /// <summary>
    /// Update and swicth which widget is displayed depending on the plane context
    /// </summary>
    void UpdateDisplayedWidget()
    {
        GameObject widgetToDisplay = null;

        //@TODO : display landing widget
        if (!Plane.RetractGear)
        {
        }
        else
        {
            // If gear is retracted, we can show weapon specific widget (if weapon safety is disabled)
            WeaponManager weaponManager = Plane.GetComponent<WeaponManager>();
            if (weaponManager)
            {
                if (weaponManager.IsEnabled)
                {
                    switch (weaponManager.CurrentWeaponMode)
                    {
                        case WeaponMode.Canon:
                        case WeaponMode.Pod_Ground:
                            // @TODO : add air_ground and canon widgets
                            break;
                        case WeaponMode.Pod_Air:
                            switch (weaponManager.CurrentSelectedWeaponType)
                            {
                                case PodItemType.Missile_IR:
                                    widgetToDisplay = irMissileWidget;
                                    break;
                                case PodItemType.Missile_Rad:
                                    widgetToDisplay = radarMissileWidget;
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        // If we switched the current displayed widget
        if (widgetToDisplay != currentDisplayedWidget)
        {
            if (currentDisplayedWidget)
                currentDisplayedWidget.SetActive(false);
            if (widgetToDisplay)
                widgetToDisplay.SetActive(true);
            currentDisplayedWidget = widgetToDisplay;
        }
    }

    // Convert a world direction (from the aircraft) to a HUD relative position
    public Vector2 WorldDirectionToScreenPosition(Vector3 worldDirection)
    {
        Vector3 PlaneRelativeDirection = Plane.transform.InverseTransformDirection(worldDirection).normalized;
        var containerTransform = canvas.GetComponent<RectTransform>();
        return new Vector2(PlaneRelativeDirection.x * containerTransform.sizeDelta.x, PlaneRelativeDirection.y * containerTransform.sizeDelta.y) / 0.39f;
    }
}
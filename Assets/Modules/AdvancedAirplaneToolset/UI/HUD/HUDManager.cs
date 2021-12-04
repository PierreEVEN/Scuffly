using UnityEngine;
using UnityEngine.UI;

// Gestion du HUD et de ses differents composants //@TODO : improve HUDManager
public class HUDManager : MonoBehaviour
{

    public float AthScale = 0.39f;

    public GameObject forwardVectorContainer;

    public GameObject velocityVectorContainer;

    public GameObject VelocityScale;
    public GameObject velocityText;

    public GameObject AltitudeScale;
    public GameObject AltitudeText;

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

        if (irMissileWidget)
            irMissileWidget.SetActive(false);
        if (radarMissileWidget)
            radarMissileWidget.SetActive(false);
    }

    // Update is called once per frame
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

        canvas.enabled = Plane.MainPower;
        alpha = Mathf.Clamp01(alpha + (Plane.MainPower ? 0.5f * Time.deltaTime : -4f * Time.deltaTime));
        canvasGroup.alpha = Mathf.Min(Plane.HudLightLevel, Mathf.Clamp01((Plane.GetCurrentPower() - 80) / 30));

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
            attitudeWidget.transform.localRotation = Quaternion.Euler(0, 0, Plane.GetRoll() * -1);
            float offset = Mathf.Asin(Plane.transform.forward.y) / (Mathf.PI / 2) * -2700;
            float rot = Plane.GetRoll() / -180 * Mathf.PI;
            attitudeWidget.transform.localPosition = new Vector2(-Mathf.Sin(rot) * offset, Mathf.Cos(rot) * offset);
        }

    }

    void UpdateDisplayedWidget()
    {
        GameObject widgetToDisplay = null;

        //@TODO : display landing widget
        if (!Plane.RetractGear)
        {
        }
        else
        {
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

        if (widgetToDisplay != currentDisplayedWidget)
        {
            if (currentDisplayedWidget)
                currentDisplayedWidget.SetActive(false);
            if (widgetToDisplay)
                widgetToDisplay.SetActive(true);
            currentDisplayedWidget = widgetToDisplay;
        }
    }


    public Vector2 WorldDirectionToScreenPosition(Vector3 worldDirection)
    {
        Vector3 PlaneRelativeDirection = Plane.transform.InverseTransformDirection(worldDirection).normalized;
        var containerTransform = canvas.GetComponent<RectTransform>();
        return new Vector2(PlaneRelativeDirection.x * containerTransform.sizeDelta.x, PlaneRelativeDirection.y * containerTransform.sizeDelta.y) / 0.39f;
    }
}
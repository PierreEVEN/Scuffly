using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle the main menu behaviour
/// </summary>
public class MenuLayoutManager : MonoBehaviour
{
    /// <summary>
    /// The different widget that can be spawned
    /// </summary>
    public GameObject OptionWidget;
    public GameObject PlayWidget;
    public GameObject HelpWidget;
    public GameObject backButton;
    public GameObject WidgetContainer;

    /// <summary>
    /// Top and bottom black borders of the menu
    /// </summary>
    public GameObject topBorder;
    public GameObject bottomBorder;

    /// <summary>
    /// A black mask to make transitions
    /// </summary>
    public GameObject BlackOverlay;

    /// <summary>
    /// Current black border height
    /// </summary>
    float screenBorders = 0;

    /// <summary>
    /// Should open menu
    /// </summary>
    bool OpenMenu = false;


    /// <summary>
    /// Start a black transition
    /// </summary>
    public void BlackScreen()
    {
        overlayOpacity = 1.1f;
    }
    /// <summary>
    /// Current opacity of the black overlay (it is decreased by the time
    /// </summary>
    float overlayOpacity = 1.2f;

    private void Update()
    {
        if (overlayOpacity > 0)
        {
            // Slowly decrease the opacity of the black overlay
            overlayOpacity -= Time.deltaTime * 0.2f;
            BlackOverlay.GetComponent<RawImage>().color = new Color(0, 0, 0, overlayOpacity);
        }

        // Move the top and bottom border
        float desiredScreenBorders = OpenMenu ? 45 : 0;
        screenBorders = screenBorders + Mathf.Clamp(desiredScreenBorders - screenBorders, -Time.deltaTime * 520, Time.deltaTime * 520);
        if (topBorder)
            topBorder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, screenBorders);
        if (bottomBorder)
            bottomBorder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, screenBorders);
    }

    /// <summary>
    /// Switch the current displayed widget
    /// </summary>
    /// <param name="widgetClass"></param>
    public void SetDisplayedWidget(GameObject widgetClass)
    {
        // Destroy the previous one
        for (int i = 0; i < WidgetContainer.transform.childCount; ++i)
            Destroy(WidgetContainer.transform.GetChild(i).gameObject);

        // And add the next one
        if (widgetClass)
            Instantiate(widgetClass, WidgetContainer.transform);

        backButton.SetActive(widgetClass ? true : false);

    }

    /// <summary>
    /// Open or close the menu
    /// </summary>
    /// <param name="open"></param>
    public void Open(bool open)
    {
        OpenMenu = open;
        if (!OpenMenu)
            SetDisplayedWidget(null);
    }

    /// <summary>
    /// Action for each button
    /// </summary>
    public void PressOptionButton() { SetDisplayedWidget(OptionWidget); }
    public void PressPlayButton() { SetDisplayedWidget(PlayWidget); }
    public void PressHelpButton() { SetDisplayedWidget(HelpWidget); }
    public void PressBackButton() { SetDisplayedWidget(null); }
    public void PressQuitButton() { Application.Quit(); }
}

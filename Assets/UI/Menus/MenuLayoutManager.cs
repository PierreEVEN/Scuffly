using UnityEngine;
using UnityEngine.UI;

public class MenuLayoutManager : MonoBehaviour
{
    public GameObject OptionWidget;
    public GameObject PlayWidget;
    public GameObject HelpWidget;

    public GameObject BlackOverlay;

    public GameObject backButton;

    public GameObject WidgetContainer;

    public GameObject topBorder;
    public GameObject bottomBorder;

    float screenBorders = 0;

    bool OpenMenu = false;

    float overlayOpacity = 1.2f;

    public void BlackScreen()
    {
        overlayOpacity = 1.1f;
    }

    private void Update()
    {
        if (overlayOpacity > 0)
        {
            overlayOpacity -= Time.deltaTime * 0.2f;
            BlackOverlay.GetComponent<RawImage>().color = new Color(0, 0, 0, overlayOpacity);
        }

        float desiredScreenBorders = OpenMenu ? 45 : 0;
        screenBorders = screenBorders + Mathf.Clamp(desiredScreenBorders - screenBorders, -Time.deltaTime * 520, Time.deltaTime * 520);
        if (topBorder)
            topBorder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, screenBorders);
        if (bottomBorder)
            bottomBorder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, screenBorders);
    }


    public void SetDisplayedWidget(GameObject widgetClass)
    {
        for (int i = 0; i < WidgetContainer.transform.childCount; ++i)
        {
            Destroy(WidgetContainer.transform.GetChild(i).gameObject);
        }
        if (widgetClass)
            Instantiate(widgetClass, WidgetContainer.transform);

        backButton.SetActive(widgetClass ? true : false);

    }

    public void Open(bool open)
    {
        OpenMenu = open;
        if (!OpenMenu)
            SetDisplayedWidget(null);
    }

    public void PressOptionButton()
    {
        SetDisplayedWidget(OptionWidget);
    }

    public void PressPlayButton()
    {
        SetDisplayedWidget(PlayWidget);
    }
    public void PressHelpButton()
    {
        SetDisplayedWidget(HelpWidget);
    }

    public void PressBackButton()
    {
        SetDisplayedWidget(null);
    }
    public void PressQuitButton()
    {
        Application.Quit();
    }
}
